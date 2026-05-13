using MudBlazor; 


namespace SOUPIShared.Misc
{
    public static class Themes
    {
        public static MudTheme SoupiTheme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "rgb(255, 167, 36)", 
                AppbarBackground = "rgb(255, 167, 36)",
            },
            PaletteDark = new PaletteDark()
            {
                Primary = "rgb(255, 182, 36)", 
                Surface = "rgba(62, 62, 71, 1)"
            }
        };
    }
}
