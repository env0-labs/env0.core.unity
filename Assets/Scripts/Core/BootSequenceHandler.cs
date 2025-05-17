using System.Collections;
using UnityEngine;

public class BootSequenceHandler : MonoBehaviour
{
    public TerminalManager terminalManager;  // Reference to TerminalManager

    // --- SBC SPECS ---
    private string[] sbcSpecs = new string[]
    {
        "env0 SBC-1 Diagnostic Utility",
        "-------------------------------",
        "CPU:      ARM Cortex-A7 1x 900MHz",
        "RAM:      512MB DDR2",
        "Storage:  4GB eMMC",
        "Board:    env0 SBC v1.2 (2023)",
        "Video:    Integrated GPU (64MB shared)",
        "Serial:   SN# SBC123456789",
        "-------------------------------",
        "Bootloader launching...",
        "",
        "Booting..."
    };

    // --- BOOT SEQUENCE CONFIG ---
    private string[] bootLines = new string[] {
        "[ OK ] Bootloader initialized",
        "[ OK ] Kernel loaded: Linux 3.12.6-sbc (armv7l)",
        "[ OK ] Mounting root filesystem (ext3)",
        "[ OK ] Remounting / read-write",
        "[ OK ] Loading device tree...",
        "[ OK ] Starting udev daemon",
        "[ OK ] Initializing virtual memory...",
        "[ OK ] Creating /dev entries",
        "[ OK ] Setting hostname to SBC_1",
        "[ OK ] Starting syslogd (busybox)",
        "[ OK ] Bringing up loopback interface",
        "[ OK ] Bringing up eth0 (wired)",
        "[ OK ] Acquiring IP via DHCP",
        "[ OK ] IP assigned: 10.10.10.99",
        "[ OK ] Starting network stack",
        "[WARN] No default gateway configured",
        "[ OK ] Loading kernel modules",
        "[ OK ] Detected 1 CPU core (ARM Cortex-A7)",
        "[ OK ] Memory check: 512MB OK",
        "[FAIL] Load microcode update — unsupported hardware",
        "[ OK ] Starting SSH service",
        "[ OK ] Starting TELNET service",
        "[SKIP] Bluetooth stack — not present",
        "[ OK ] Checking disk integrity (/dev/mmcblk0)",
        "[ OK ] tmpfs mounted on /run",
        "[ OK ] Mounting /mnt/usb — no media present",
        "[ OK ] Starting watchdog timer",
        "[ OK ] Loading TTY interfaces",
        "[ OK ] Mounting user partition",
        "[ OK ] Sourcing boot scripts (/etc/init.d)",
        "[ OK ] Configuring time zone (UTC)",
        "[ OK ] Starting user login service",
        "[FAIL] journalctl daemon not available — skipping logs",
        "[ OK ] Binding /bin/sh to TTY1",
        "[ OK ] Finalizing runtime state",
        "[ OK ] systemd: user mode emulation (partial)",
        "[ OK ] Boot completed in 18.532s",
        "[ OK ] SBC_1 ready"
    };

    public IEnumerator StartBootSequence()
    {
        // Set mode to Boot at the start of the boot sequence
        terminalManager.SetMode(TerminalManager.TerminalMode.Boot);
        terminalManager.ClearBuffer();

        // Initial "Booting..." pause (5 seconds)
        terminalManager.AddLine("Booting...");
        float bootPause = 0f;
        while (bootPause < 5f)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                goto BootSkip;
            bootPause += Time.deltaTime;
            yield return null;
        }

        // Print SBC Specs
        foreach (var line in sbcSpecs)
        {
            terminalManager.AddLine(line);

            // Special pause after "Bootloader launching..."
            if (line == "Bootloader launching...")
            {
                float bootloaderPause = 0f;
                while (bootloaderPause < 3f)
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                        goto BootSkip;
                    bootloaderPause += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                float timer = 0f;
                float delay = 0.15f; // Fast for specs
                while (timer < delay)
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                        goto BootSkip;
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
        }

        // Print Boot Lines with smart timing and smooth spinner animation
        string[] spinner = new string[] { "|", "/", "-", "\\" };
        foreach (var line in bootLines)
        {
            terminalManager.AddLine(line);

            float delay;
            if (line.StartsWith("[ OK ]"))
                delay = Random.Range(0.09f, 0.49f);
            else if (line.StartsWith("[SKIP]"))
                delay = Random.Range(0.10f, 0.18f);
            else if (line.StartsWith("[WARN]"))
                delay = Random.Range(0.65f, 1f);
            else if (line.StartsWith("[FAIL]"))
                delay = Random.Range(0.90f, 2f);
            else
                delay = Random.Range(0.12f, 1.5f);

            int spinnerIndex = 0;
            float spinnerTimer = 0f;
            float spinnerInterval = 0.12f; // <-- Set spinner speed here!
            float timer = 0f;
            while (timer < delay)
            {
                spinnerTimer += Time.deltaTime;
                if (spinnerTimer >= spinnerInterval)
                {
                    terminalManager.buffer[terminalManager.buffer.Count - 1] = line + " " + spinner[spinnerIndex];
                    terminalManager.RenderTerminal();
                    spinnerIndex = (spinnerIndex + 1) % spinner.Length;
                    spinnerTimer = 0f;
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                    goto BootSkip;
                timer += Time.deltaTime;
                yield return null;
            }
            // After waiting, finalize line with spinner removed
            terminalManager.buffer[terminalManager.buffer.Count - 1] = line;
            terminalManager.RenderTerminal();
        }

    BootSkip:
        // Print any missed specs and boot lines
        foreach (var line in sbcSpecs)
        {
            if (!terminalManager.buffer.Contains(line))
                terminalManager.AddLine(line);
        }
        foreach (var line in bootLines)
        {
            if (!terminalManager.buffer.Contains(line))
                terminalManager.AddLine(line);
        }

        terminalManager.AddLine("Press any key to continue...");

        // Wait until ANY key is pressed
        while (!Input.anyKeyDown)
            yield return null;

        // Set mode to Login when the boot sequence is done and player advances
        terminalManager.SetMode(TerminalManager.TerminalMode.Login);
    }
}
