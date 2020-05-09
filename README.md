# Simple OTP Bruteforce

This is a very simple C# demo program to do OTP bruteforce attack for changing the target user password,
assuming that the target endpoint does not have rate limiting againts bruteforce attack.

**Note:** Please do not use this code for illegal purpose.
I have no responsibility or liability for what you do using this code.

## Requirements

To modify and build the code, you need .NET Core 3.1.

## How to use
Modify and build this code, then run it using command prompt or terminal.

This program takes three parameters, the first parameter is the target email address, 
the second parameter is the new password, and the third is device id.

On Windows:
```
> SimpleOTPBruteForce.exe "target.user@example.com" "password123" "4d1e0fc123"
```

On Linux:
```
> ./SimpleOTPBruteForce "target.user@example.com" "password123" "4d1e0fc123"
```