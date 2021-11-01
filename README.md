# CreateFileSignature

Welcome to "Create signature" program.
This program generates SHA-256 signature of file chunks.
You can run this program with command line parameters or set them during execution.

Parameters list:
1: Path to the file.
2. Chunk size (bytes by default). You can specify measure after number (without spaces): b - bytes, k - killobytes, m - megabytes. 
3. Should we order signature results according chunks index: y/n (n by default). Using y parameter can affect performance.

Example of commands:

Split Test.txt file to 100mb chunks and get Sha-256 signature for every chunk:
```
CreateFileSignature Test.txt 100m
```
Setup parameters during execution:
```
CreateFileSignature
```
Split Test.txt file to 100000 bytes chunks and get Sha-256 signature for every chunk with ordered output:
```
CreateFileSignature Test.txt 100000 y
```
