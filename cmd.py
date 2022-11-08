#! /usr/bin/env python

import sys
import subprocess

inputs = sys.argv

command = inputs[1]

try:
    if (command == "build"):
        print("[Python CMD]  Project is building.")
        subprocess.run(["dotnet", "build"])
    elif (command == "run"):
        print("[Python CMD]  Project is running.")
        subprocess.run(["dotnet", "run", "--project", "src/api"])
    elif (command == "wrun"):
        print("[Python CMD]  Project is running in watch mode.")
        subprocess.run(["dotnet", "watch", "run", "--project", "src/api"])
    else:
        print("[Python CMD]  Didn't Match Anything.")
except KeyboardInterrupt:
    print("[Python CMD] Program interrupted.")
