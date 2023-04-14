# AsmWorker
The AsmWorker module provides a simple way to create a worker thread in a remote process that executes x86 assembly code.


## Usage
To use the AsmWorker module, simply call the CreateAsmWorker function with the following parameters:
- **hProcess** - the handle to the target process in which to create the worker thread
- **asmBytes** - the x86 assembly code to execute in the worker thread
- **asmSize** - the size of the assembly code in bytes

The function returns a boolean value indicating whether the worker thread was successfully created and executed.

Here's an example of how to use the AsmWorker module:
```vb.net
'Get a process object for the target process
Dim targetProcess As Process = Process.GetProcessesByName("notepad").FirstOrDefault()

If targetProcess Is Nothing Then
    Console.WriteLine("Target process not found.")
    Return
End If

'Open the process with PROCESS_CREATE_THREAD and PROCESS_VM_OPERATION access
Dim hProcess As IntPtr = OpenProcess(&H1 Or &H8, False, targetProcess.Id)

If hProcess = IntPtr.Zero Then
    Console.WriteLine("Failed to open process.")
    Return
End If

'Example assembly code to write a message box to the target process
Dim asmCode As Byte() = {
    &H68, &H0, &H0, &H0, &H0,   'push 0
    &H68, &H0, &H0, &H0, &H0,   'push 0
    &H68, &H0, &H0, &H0, &H0,   'push 0
    &H68, &H0, &H0, &H0, &H0,   'push 0
    &H68, &H0, &H0, &H0, &H0,   'push 0
    &HB8, &H0, &H0, &H0, &H0,   'mov eax, MessageBoxA
    &HFF, &HD0,                 'call eax
    &HC3                        'ret
}

'Call the CreateAsmWorker function to execute the assembly code in the target process
If Not CreateAsmWorker(hProcess, asmCode, asmCode.Length) Then
    Console.WriteLine("Failed to create asm worker.")
Else
    Console.WriteLine("Successfully created asm worker.")
End If

'Close the handle to the target process
CloseHandle(hProcess)
```


## Dependencies
The **AsmWorker** module depends on the **System.Runtime.InteropServices** namespace, which is part of the .NET Framework. This namespace provides interop services that enable managed code to call unmanaged code, and vice versa.

In addition, the module uses several functions from the kernel32.dll library, which is part of the Windows API. These functions are:

- **VirtualAllocEx**: allocates memory within the virtual address space of a specified process.
- **WriteProcessMemory**: writes data to an area of memory in a specified process.
- **CreateRemoteThread**: creates a thread that runs in the virtual address space of another process.

Therefore, the module indirectly depends on the Windows operating system, which provides the Windows API and the **kernel32.dll** library.


## Contributing
If you find any issues with this module or have suggestions for improvements, feel free to open an issue or submit a pull request.


## License
This module is licensed under the MIT License.
