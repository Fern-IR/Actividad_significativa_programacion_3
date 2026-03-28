import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const rootNorm = path.dirname(fileURLToPath(import.meta.url));

function shouldProcess(filePath) {
  const rel = path.relative(rootNorm, filePath);
  const parts = rel.split(path.sep);
  if (parts.includes("bin") || parts.includes("obj") || parts.includes(".vs")) return false;
  const name = path.basename(filePath);
  if (name.endsWith(".g.cs") || name.endsWith(".g.i.cs")) return false;
  if (name.endsWith("Designer.cs")) return false;
  const ext = path.extname(filePath).toLowerCase();
  return ext === ".cs" || ext === ".xaml";
}

function findLineCommentStart(line) {
  let i = 0;
  const n = line.length;
  let inStr = false;
  let verbatim = false;
  let chQuote = null;
  while (i < n - 1) {
    const c = line[i];
    const d = line[i + 1];
    if (!inStr) {
      if (c === "@" && d === '"') {
        inStr = true;
        verbatim = true;
        i += 2;
        continue;
      }
      if (c === '"') {
        inStr = true;
        verbatim = false;
        chQuote = '"';
        i += 1;
        continue;
      }
      if (c === "'" && (i === 0 || !/[a-zA-Z0-9_]/.test(line[i - 1]))) {
        inStr = true;
        chQuote = "'";
        i += 1;
        continue;
      }
      if (c === "/" && d === "/") return i;
      i += 1;
      continue;
    }
    if (verbatim) {
      if (c === '"' && d === '"') {
        i += 2;
        continue;
      }
      if (c === '"') {
        inStr = false;
        verbatim = false;
        i += 1;
        continue;
      }
      i += 1;
      continue;
    }
    if (chQuote === '"') {
      if (c === "\\") {
        i += 2;
        continue;
      }
      if (c === '"') {
        inStr = false;
        i += 1;
        continue;
      }
      i += 1;
      continue;
    }
    if (chQuote === "'") {
      if (c === "\\") {
        i += 2;
        continue;
      }
      if (c === "'") {
        inStr = false;
        i += 1;
        continue;
      }
      i += 1;
      continue;
    }
  }
  return -1;
}

function fixCsLine(line) {
  if (!line.includes("//")) return line;
  const idx = findLineCommentStart(line);
  if (idx < 0) return line;
  const prefix = line.slice(0, idx);
  const commentPart = line.slice(idx);
  const m = commentPart.match(/^(\/\/+)(.*)$/);
  if (!m) return line;
  const slashes = m[1];
  let body = m[2];
  const trailingMatch = body.match(/\r?\n$/);
  const trailingNl = trailingMatch ? trailingMatch[0] : "";
  body = body.slice(0, body.length - trailingNl.length);
  const stripped = body.replace(/[ \t]+$/g, "");
  const newBody = stripped.replace(/\.\s*$/, "");
  return prefix + slashes + newBody + trailingNl;
}

function fixXamlLine(line) {
  if (!line.includes("<!--") || !line.includes("-->")) return line;
  let out = "";
  let pos = 0;
  while (true) {
    const a = line.indexOf("<!--", pos);
    if (a < 0) {
      out += line.slice(pos);
      break;
    }
    out += line.slice(pos, a);
    const b = line.indexOf("-->", a + 4);
    if (b < 0) {
      out += line.slice(a);
      break;
    }
    const inner = line.slice(a + 4, b);
    const s = inner.replace(/[ \t]+$/g, "");
    const newS = s.replace(/\.\s*$/, "");
    const tailSpaces = inner.slice(s.length);
    out += "<!--" + newS + tailSpaces + "-->";
    pos = b + 3;
  }
  return out;
}

function splitLinesKeepNewlines(text) {
  const lines = [];
  let i = 0;
  while (i < text.length) {
    const n = text.indexOf("\n", i);
    if (n === -1) {
      lines.push(text.slice(i));
      break;
    }
    lines.push(text.slice(i, n + 1));
    i = n + 1;
  }
  return lines;
}

function walk(dir, acc) {
  for (const ent of fs.readdirSync(dir, { withFileTypes: true })) {
    const p = path.join(dir, ent.name);
    if (ent.isDirectory()) walk(p, acc);
    else acc.push(p);
  }
}

const all = [];
walk(rootNorm, all);
const modified = [];
for (const p of all) {
  if (!shouldProcess(p)) continue;
  let text = fs.readFileSync(p, "utf8");
  const orig = text;
  const ext = path.extname(p).toLowerCase();
  const lines = splitLinesKeepNewlines(text);
  if (ext === ".xaml") {
    text = lines.map(fixXamlLine).join("");
  } else {
    text = lines.map(fixCsLine).join("");
  }
  if (text !== orig) {
    fs.writeFileSync(p, text, "utf8");
    modified.push(path.relative(rootNorm, p));
  }
}
modified.sort();
for (const m of modified) console.log(m);
console.log("---");
console.log("count", modified.length);
