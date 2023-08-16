
using Color = System.Drawing.Color;

namespace SharedDefinitions;

public static class Definition
{

    public static int MessageGetDefault { get; } = 40;
    public static int MaxUsernameLength { get; } = 10;
    public static int MaxMessageBytes { get; } = 130000;
    public static int DefaultPortCommunication { get; } = 5000;
    public static int DefaultPortFiles { get; } = 8081;
    public static string DefaultDomain { get; } = "localhost";

    



    #region colors

    public static System.Drawing.Color GetRandomUserColor()
    {
        var random = new Random();
        int index = random.Next(0, UserColors.Length);
        return UserColors[index];
    }
    
    
    // standard usercolors (no admin [red])
    public static System.Drawing.Color Orange { get; } = Color.FromArgb(245, 155, 0);
    public static System.Drawing.Color Yellow { get; } = System.Drawing.Color.FromArgb(255, 255, 0);
    public static System.Drawing.Color Red { get; } = System.Drawing.Color.FromArgb(229, 57, 53);
    public static System.Drawing.Color Pink { get; } = System.Drawing.Color.FromArgb(216, 27, 96);
    public static System.Drawing.Color Rose { get; } = System.Drawing.Color.FromArgb(255, 105, 180);
    public static System.Drawing.Color Purple { get; } = System.Drawing.Color.FromArgb(142, 36, 170);
    public static System.Drawing.Color DeepPurple { get; } = System.Drawing.Color.FromArgb(94, 53, 177);
    public static System.Drawing.Color Indigo { get; } = System.Drawing.Color.FromArgb(57, 73, 171);
    public static System.Drawing.Color Blue { get; } = System.Drawing.Color.FromArgb(30, 136, 229);
    public static System.Drawing.Color Cyan { get; } = System.Drawing.Color.FromArgb(0, 188, 212);
    public static System.Drawing.Color Teal { get; } = System.Drawing.Color.FromArgb(0, 137, 123);
    public static System.Drawing.Color Green { get; } = System.Drawing.Color.FromArgb(67, 160, 71);
    public static System.Drawing.Color LightGreen { get; } = System.Drawing.Color.FromArgb(124, 179, 66); 
    
    public static System.Drawing.Color[] UserColors = new[]
        { Orange,Yellow,Pink,Rose,Purple,DeepPurple,Indigo,Blue,Cyan,Teal,Green,LightGreen};
    
    public static System.Drawing.Color[] ThemeColors = new[]
        { Orange,Yellow,Pink,Rose,Purple,DeepPurple,Indigo,Blue,Cyan,Teal,Green,LightGreen};
    
    public static System.Drawing.Color[] AllColors = new[]
        { Orange,Yellow,Pink,Rose,Purple,DeepPurple,Indigo,Blue,Cyan,Teal,Green,LightGreen};

    public static Dictionary<string,string> NameColorRgbDic { get; set; } = new Dictionary<string, string>()
    {
        { Orange.Name,nameof(Orange) },
        { Yellow.Name,nameof(Yellow) },
        { Red.Name,nameof(Red)  },
        { Pink.Name,nameof(Pink) },
        { Rose.Name,nameof(Rose) },
        { Purple.Name,nameof(Purple) },
        { DeepPurple.Name,nameof(DeepPurple) },
        { Indigo.Name,nameof(Indigo) },
        { Blue.Name,nameof(Blue) },
        { Cyan.Name,nameof(Cyan) },
        { Teal.Name,nameof(Teal) },
        { Green.Name,nameof(Green) },
        { LightGreen.Name,nameof(LightGreen) },
    };



    #endregion


}