# My .net interoperability workspace. Not very useful yet ...

My .net interoperability workspace.
It might evolve in the future to interop wrapper for some functionality listed here:

https://msdn.microsoft.com/en-us/library/windows/desktop/ms679321%28v=vs.85%29.aspx


Hope to create some useful demo of how to use interop and what are best practices while using it.


###How to create simple interop app.

TestInterop project contains simple interop demo. Application flashes window when the button is pressed.
Demo uses two functions FlashWindow and FlashWindowEx. Both functions are located in User32 dll.

To use unmanaged code function in .net we have to create function prototype in .net code and then add DllImport attribute wit specified dll name.

We can find function prototype in documentanion:

https://msdn.microsoft.com/en-us/library/windows/desktop/ms679346%28v=vs.85%29.aspx

https://msdn.microsoft.com/en-us/library/windows/desktop/ms679347%28v=vs.85%29.aspx

As you can see we have to adjust types like HWND or DWORD  to .net types. For FlashWindowEx function we also have to create FLASHWINFO structure which is expected as an parameter type.

https://msdn.microsoft.com/en-us/library/windows/desktop/ms679348%28v=vs.85%29.aspx

Our new types:

HWND -> IntPtr

DWORD -> short

and our FLASHWINFO structure:

```cs

[StructLayout(LayoutKind.Sequential)]
struct FLASHWINFO
{
  public short cbSize;
  public IntPtr hwnd;
  public uint dwFlags;
  public short uCount;
  public uint dwTimeout;
}

```

StructLayout attribute is used to prevent from changing the struct types alignment.
Read more about it in documentation:

https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.structlayoutattribute%28v=vs.71%29.aspx

Our functions:

```cs

  [DllImport("user32")]
  extern static bool FlashWindow(IntPtr hWnd, bool bInvert);
        
  [DllImport("user32")]
  extern static bool FlashWindowEx(out FLASHWINFO pfwi);

```

extern keyword means that the function body is somewhere else.
Pay attention that pfwi is a pointer to the  FLASHWINFO structure. So in our prototype we have to specify it like an out parameter.

Calling our new functions is quite simple:

```cs
private void Button_Click(object sender, RoutedEventArgs e)
{
    Window window = Window.GetWindow(this);
    var wih = new WindowInteropHelper(window);
    IntPtr hWnd = wih.Handle;

    FlashWindow(hWnd, true);
}
```

```cs
private void Button_Click_1(object sender, RoutedEventArgs e)
{
    Window window = Window.GetWindow(this);
    var wih = new WindowInteropHelper(window);
    IntPtr hWnd = wih.Handle;

    var s = System.Runtime.InteropServices.Marshal.SizeOf(typeof(FLASHWINFO));

    FLASHWINFO fi = new FLASHWINFO()
    {
        cbSize = (short)s,
        hwnd = hWnd,
        dwFlags = 0x00000003,
        uCount = 5,
        dwTimeout = 0
    };
    FlashWindowEx(out fi);
}
```

To get window pointer (window handle) we use WindowInteropHelper which is located in System.Windows.Interop;

In the second case we need FLASHWINFO struct size in bytes. 
We can count it by ourselves (2 + 8 + 4 + 2 + 4 in my case) or we can use Marshal.SizeOf function from System.Runtime.InteropServices namespace.

Enjoy!
