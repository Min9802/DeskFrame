using DeskFrame;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Interop;
using System.Xml.Linq;
public class InstanceController
{
    public static string appName = "DeskFrame";
    public List<Instance> Instances = new List<Instance>();
    public RegistryHelper reg = new RegistryHelper(appName);
    public List<DeskFrameWindow> _subWindows = new List<DeskFrameWindow>();

    public void WriteOverInstanceToKey(Instance instance, string oldKey)
    {

        try
        {
            Debug.WriteLine($"old: {oldKey}\t{instance.Name}");

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@$"SOFTWARE\{appName}\Instances\{instance.Name}"))
            {
                key.SetValue("Folder", instance.Folder!);
                key.SetValue("Name", instance.Name!);
                key.SetValue("PosX", instance.PosX!);
                key.SetValue("PosY", instance.PosY!);
                key.SetValue("Width", instance.Width!);
                key.SetValue("Height", instance.Height!);
                key.SetValue("Minimized", instance.Minimized!);
                key.SetValue("ShowHiddenFiles", instance.ShowHiddenFiles!);
                key.SetValue("IsLocked", instance.IsLocked!);
                key.SetValue("TitleBarColor", instance.TitleBarColor!);
                key.SetValue("TitleTextColor", instance.TitleTextColor!);
            }
            Registry.CurrentUser.DeleteSubKey(@$"SOFTWARE\{appName}\Instances\{oldKey}", throwOnMissingSubKey: false);
        }
        catch { }
    }
    private void InitDetails()
    {
        double opacity = 60;

        if (reg.KeyExistsRoot("isBlack"))
        {
            ChangeIsBlack((bool)reg.ReadKeyValueRoot("isBlack"));         
        }
        if (reg.KeyExistsRoot("Opacity")) {

            ChangeBackgroundOpacity(Int32.Parse(reg.ReadKeyValueRoot("Opacity").ToString()!));
        }
        else
        {
            ChangeBackgroundOpacity(60);
        }
        if (reg.KeyExistsRoot("blurBackground"))
        {
            ChangeBlur((bool)reg.ReadKeyValueRoot("blurBackground"));
        }
    }
    public void WriteInstanceToKey(Instance instance)
    {
        if (string.IsNullOrEmpty(instance.Name))
        {
            return;
        }
        try
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(instance.GetKeyLocation()))
            {
                key.SetValue("Name", instance.Name!);
                key.SetValue("PosX", instance.PosX!);
                key.SetValue("PosY", instance.PosY!);
                key.SetValue("Width", instance.Width!);
                key.SetValue("Height", instance.Height!);
                key.SetValue("Minimized", instance.Minimized!);
                key.SetValue("Folder", instance.Folder!);
                key.SetValue("ShowHiddenFiles", instance.ShowHiddenFiles!);
                key.SetValue("IsLocked", instance.IsLocked!);
                key.SetValue("TitleBarColor", instance.TitleBarColor!);
                key.SetValue("TitleTextColor", instance.TitleTextColor!);
                key.SetValue("TitleTextAlignment", instance.TitleTextAlignment.ToString());
            }
        }
        catch { }
    }

    public void AddInstance()
    {
        var existingEmptyInstance = Instances.FirstOrDefault(instance => instance.Name == "empty");

        if (existingEmptyInstance != null)
        {
            Instances.Remove(existingEmptyInstance);
        }

        Instances.Add(new Instance("empty"));
        MainWindow._controller.WriteInstanceToKey(Instances.Last());
        var subWindow = new DeskFrameWindow(Instances.Last());
        _subWindows.Add(subWindow);
        subWindow.Show();
        InitDetails();
    }

    public void ChangeBlur(bool toBlur)
    {
        foreach (DeskFrameWindow window in _subWindows)
        {
            window.BackgroundType(toBlur);
        }
    }
    public void ChangeIsBlack(bool isBlack)
    {
        foreach (DeskFrameWindow window in _subWindows)
        {
            window.ChangeIsBlack(isBlack);
        }
    }
    public void ChangeBackgroundOpacity(int num)
    {
        foreach (DeskFrameWindow window in _subWindows)
        {
            window.ChangeBackgroundOpacity(num);
        }
    }
    public void InitInstances()
    {
        Debug.WriteLine("Init...");
        try
        {
            using (RegistryKey instancesKey = Registry.CurrentUser.OpenSubKey(@$"SOFTWARE\{appName}\Instances")!)
            {
                if (instancesKey != null)
                {
                    string[] instanceNames = instancesKey.GetSubKeyNames();
                    Debug.WriteLine($"\n");

                    foreach (var item in instanceNames)
                    {
                        Debug.WriteLine($"subkeyname: {item}");
                    }

                    foreach (string instance in instanceNames)
                    {

                        Debug.WriteLine($"instanceNames: {instance}");


                        using (RegistryKey instanceKey = Registry.CurrentUser.OpenSubKey(@$"SOFTWARE\{appName}\Instances\{instance}")!)
                        {
                            Debug.WriteLine("valid");
                            if (instanceKey != null)
                            {
                                Instance temp = new Instance("");
                                Debug.WriteLine("valied 2");

                                foreach (var valueName in instanceKey.GetValueNames())   // Read all values under the current subkey
                                {
                                    object value = instanceKey.GetValue(valueName)!;

                                    if (value != null)
                                    {
                                        switch (valueName)
                                        {
                                            case "PosX":
                                                if (double.TryParse(value.ToString(), out double parsedPosX))
                                                {
                                                    temp.PosX = parsedPosX;
                                                    Debug.WriteLine($"PosX added: {temp.PosX}");
                                                }
                                                else
                                                {
                                                    Debug.WriteLine("Failed to parse PosX.");
                                                }
                                                break;
                                            case "PosY":
                                                if (double.TryParse(value.ToString(), out double parsedPosY))
                                                {
                                                    temp.PosY = parsedPosY;
                                                    Debug.WriteLine($"PosY added: {temp.PosY}");
                                                }
                                                else
                                                {
                                                    Debug.WriteLine("Failed to parse PosY.");
                                                }
                                                break;

                                            case "Width":
                                                if (double.TryParse(value.ToString(), out double parsedWidth))
                                                {
                                                    temp.Width = parsedWidth;
                                                    Debug.WriteLine($"Width added: {temp.Width}");
                                                }
                                                else
                                                {
                                                    Debug.WriteLine("Failed to parse Width.");
                                                }
                                                break;

                                            case "Height":
                                                if (double.TryParse(value.ToString(), out double parsedHeight))
                                                {
                                                    temp.Height = parsedHeight;
                                                }

                                                break;

                                            case "Name":
                                                temp.Name = value.ToString()!;
                                                Debug.WriteLine($"Name added\t{temp.Name}");
                                                break;

                                            case "Folder":
                                                Debug.WriteLine("+trest:  " + value.ToString());
                                                temp.Folder = value.ToString()!;
                                                Debug.WriteLine($"Folder added\t{temp.Folder}");
                                                break;
                                            case "Minimized":
                                                temp.Minimized = bool.Parse(value.ToString()!);
                                                Debug.WriteLine($"Minimized added\t{temp.Minimized}");
                                                break;
                                            case "ShowHiddenFiles":
                                                temp.ShowHiddenFiles = bool.Parse(value.ToString()!);
                                                Debug.WriteLine($"ShowHiddenFiles added\t{temp.ShowHiddenFiles}");
                                                break;
                                            case "IsLocked":
                                                temp.IsLocked = bool.Parse(value.ToString()!);
                                                Debug.WriteLine($"IsLocked added\t{temp.IsLocked}");
                                                break;
                                            case "TitleBarColor":
                                                temp.TitleBarColor = value.ToString()!;
                                                Debug.WriteLine($"TitleBarColor added\t{temp.TitleBarColor}");
                                                break;
                                            case "TitleTextColor":
                                                temp.TitleTextColor = value.ToString()!;
                                                Debug.WriteLine($"TitleTextColor added\t{temp.TitleTextColor}");
                                                break;
                                            case "TitleTextAlignment":
                                                if (Enum.TryParse<System.Windows.HorizontalAlignment>(value.ToString()!, out var alignment))
                                                {
                                                    temp.TitleTextAlignment = alignment;
                                                }
                                                break;
                                            default:
                                                Debug.WriteLine($"Unknown value: {valueName}");
                                                break;
                                        }
                                    }

                                }
                                if (temp.Name != "empty")
                                {
                                    Instances.Add(temp);
                                }

                            }
                            else
                            {
                                Debug.WriteLine("instance not valid");
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("try add an empty");
                    Instances.Add(new Instance("empty"));
                    MainWindow._controller.WriteInstanceToKey(Instances[0]);

                }
            }
            Debug.WriteLine("Showing windows...");
            foreach (var Instance in Instances)
            {
                var subWindow = new DeskFrameWindow(Instance);
                _subWindows.Add(subWindow);
                subWindow.Show();
                InitDetails();
            }

            if (Instances.Count == 0)
            {
                AddInstance();
            }
            Debug.WriteLine("Showing windows DONE");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR reading key: {ex.Message}");
        }
    }
}