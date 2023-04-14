Imports System.Runtime.InteropServices

Module AsmWorker

    'Import the VirtualAllocEx and WriteProcessMemory functions from kernel32.dll
    Private Declare Function VirtualAllocEx Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As UInt32, ByVal flAllocationType As UInt32, ByVal flProtect As UInt32) As IntPtr
    Private Declare Function WriteProcessMemory Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer As Byte(), ByVal nSize As UInt32, ByRef lpNumberOfBytesWritten As UInt32) As Boolean
    Private Declare Function CreateRemoteThread Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal lpThreadAttributes As IntPtr, ByVal dwStackSize As UInt32, ByVal lpStartAddress As IntPtr, ByVal lpParameter As IntPtr, ByVal dwCreationFlags As UInt32, ByRef lpThreadId As IntPtr) As IntPtr
    Private Declare Function WaitForSingleObject Lib "kernel32.dll" (ByVal hHandle As IntPtr, ByVal dwMilliseconds As UInt32) As UInt32
    Private Declare Function CloseHandle Lib "kernel32.dll" (ByVal hObject As IntPtr) As Boolean
    Private Declare Function VirtualFreeEx Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As UInt32, ByVal dwFreeType As UInt32) As Boolean
                
    Public Function CreateAsmWorker(ByVal hProcess As IntPtr, ByVal asmBytes As Byte(), ByVal asmSize As Integer) As Boolean
        'Allocate memory for the assembly code in the target process
        Dim pRemoteCode As IntPtr = VirtualAllocEx(hProcess, IntPtr.Zero, asmSize, &H1000 Or &H2000, &H40)

        If pRemoteCode = IntPtr.Zero Then
            Return False
        End If

        'Write the assembly code to the allocated memory
        Dim bytesWritten As UInt32 = 0
        If Not WriteProcessMemory(hProcess, pRemoteCode, asmBytes, asmSize, bytesWritten) Then
            Return False
        End If

        'Start a worker thread on the allocated memory
        Dim hThread As IntPtr = CreateRemoteThread(hProcess, IntPtr.Zero, 0, pRemoteCode, IntPtr.Zero, 0, IntPtr.Zero)

        If hThread = IntPtr.Zero Then
            Return False
        End If

        'Wait for the worker thread to finish executing
        If WaitForSingleObject(hThread, INFINITE) <> WAIT_OBJECT_0 Then
            Return False
        End If

        'Close the worker thread handle
        CloseHandle(hThread)

        'Free the allocated memory
        VirtualFreeEx(hProcess, pRemoteCode, 0, &H8000)

        Return True
    End Function

End Module
