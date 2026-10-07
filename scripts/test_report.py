#!/usr/bin/env python3
"""Turns a .trx test result file (from `dotnet test --logger trx`) into a Markdown report.

usage: test_report.py RESULTS.trx OUTPUT.md [--version X.Y.Z] [--commit SHA]

Standard library only, so it runs on any CI runner. Exits 0 even when tests failed:
the test step already decides the build status, this only reports.
"""
import argparse
import datetime
import re
import sys
import xml.etree.ElementTree as ET
from collections import defaultdict

NS = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}


def parse_duration(text):
    """'00:00:01.2340000' -> seconds."""
    if not text:
        return 0.0
    h, m, s = text.split(":")
    return int(h) * 3600 + int(m) * 60 + float(s)


def fmt_seconds(seconds):
    return f"{seconds * 1000:.0f} ms" if seconds < 1 else f"{seconds:.2f} s"


def load(path):
    root = ET.parse(path).getroot()
    classes = {}
    for unit in root.iterfind(".//t:UnitTest", NS):
        method = unit.find("t:TestMethod", NS)
        if method is not None:
            classes[unit.get("id")] = method.get("className", "").split(",")[0]

    results = []
    for r in root.iterfind(".//t:UnitTestResult", NS):
        message = r.findtext(".//t:ErrorInfo/t:Message", default="", namespaces=NS)
        trace = r.findtext(".//t:ErrorInfo/t:StackTrace", default="", namespaces=NS)
        results.append({
            "name": r.get("testName", ""),
            "class": classes.get(r.get("testId"), "(unknown)"),
            "outcome": r.get("outcome", "Unknown"),
            "seconds": parse_duration(r.get("duration")),
            "message": message.strip(),
            "trace": trace.strip(),
        })

    times = root.find("t:Times", NS)
    wall = None
    if times is not None and times.get("start") and times.get("finish"):
        # trx timestamps carry 7 fractional digits; fromisoformat takes at most 6
        def parse(ts):
            return datetime.datetime.fromisoformat(re.sub(r"(\.\d{6})\d+", r"\1", ts))
        try:
            wall = (parse(times.get("finish")) - parse(times.get("start"))).total_seconds()
        except ValueError:
            wall = None
    return results, wall


def area(class_name):
    """NestLight.Tests.Languages.CssTokenizerTests -> Languages"""
    parts = class_name.split(".")
    return parts[2] if len(parts) > 3 and parts[:2] == ["NestLight", "Tests"] else "Other"


def bucket(outcome):
    return {"Passed": "passed", "Failed": "failed"}.get(outcome, "skipped")


def tally(rows):
    out = {"total": 0, "passed": 0, "failed": 0, "skipped": 0, "seconds": 0.0}
    for r in rows:
        out["total"] += 1
        out[bucket(r["outcome"])] += 1
        out["seconds"] += r["seconds"]
    return out


def row(label, t):
    return f"| {label} | {t['total']} | {t['passed']} | {t['failed']} | {t['skipped']} | {fmt_seconds(t['seconds'])} |"


HEADER = "| | Total | Passed | Failed | Skipped | Time |\n|---|---:|---:|---:|---:|---:|"


def render(results, wall, version, commit):
    total = tally(results)
    ok = total["failed"] == 0 and total["total"] > 0
    lines = ["# NestLight test report", ""]
    lines.append(f"**Result: {'PASSED' if ok else 'FAILED'}**" + (f" · version `{version}`" if version else "")
                 + (f" · commit `{commit[:7]}`" if commit else ""))
    lines.append(f"Generated {datetime.datetime.now(datetime.timezone.utc):%Y-%m-%d %H:%M UTC}"
                 + (f" · wall time {fmt_seconds(wall)}" if wall is not None else ""))
    lines += ["", "## Summary", "", HEADER.replace("| |", "| Tests |", 1), row("All", total), ""]

    by_area = defaultdict(list)
    by_class = defaultdict(list)
    for r in results:
        by_area[area(r["class"])].append(r)
        by_class[r["class"]].append(r)

    lines += ["## By area", "", HEADER.replace("| |", "| Area |", 1)]
    lines += [row(a, tally(rs)) for a, rs in sorted(by_area.items())]
    lines += ["", "## By test class", "", HEADER.replace("| |", "| Class |", 1)]
    lines += [row(c.split(".", 2)[-1], tally(rs)) for c, rs in sorted(by_class.items())]

    failed = [r for r in results if r["outcome"] == "Failed"]
    lines += ["", "## Failures", ""]
    if not failed:
        lines.append("None.")
    for r in failed:
        lines += [f"### {r['class'].split('.', 2)[-1]} › {r['name']}", "", "```", r["message"] or "(no message)"]
        if r["trace"]:
            lines += ["", r["trace"]]
        lines += ["```", ""]

    skipped = [r for r in results if bucket(r["outcome"]) == "skipped"]
    if skipped:
        lines += ["", "## Skipped", ""] + [f"- {r['class'].split('.', 2)[-1]} › {r['name']} ({r['outcome']})" for r in skipped]

    slowest = sorted(results, key=lambda r: r["seconds"], reverse=True)[:10]
    lines += ["", "## Slowest tests", "", "| Test | Time |", "|---|---:|"]
    lines += [f"| {r['class'].split('.', 2)[-1]} › {r['name']} | {fmt_seconds(r['seconds'])} |" for r in slowest]
    return "\n".join(lines) + "\n", ok


def main():
    p = argparse.ArgumentParser()
    p.add_argument("trx")
    p.add_argument("output")
    p.add_argument("--version")
    p.add_argument("--commit")
    args = p.parse_args()

    results, wall = load(args.trx)
    if not results:
        print("no test results found in", args.trx, file=sys.stderr)
    text, _ = render(results, wall, args.version, args.commit)
    with open(args.output, "w", encoding="utf-8") as f:
        f.write(text)
    print(f"{len(results)} results -> {args.output}")


if __name__ == "__main__":
    main()
