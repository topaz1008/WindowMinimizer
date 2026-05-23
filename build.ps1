Write-Host "Preparing build..." -ForegroundColor Cyan

# 1. Generate icon.ico dynamically so the compiler can embed it into the .exe
$iconGenCode = @"
using System.Drawing;
using System.IO;

public class IconGenerator {
    public static void CreateIco() {
        // Draw a 32x32 blue square with a white minus for the desktop/exe icon
        using (Bitmap bmp = new Bitmap(32, 32))
        using (Graphics g = Graphics.FromImage(bmp)) {
            g.Clear(Color.Transparent);
            g.FillRectangle(Brushes.DodgerBlue, 4, 4, 24, 24);
            g.DrawLine(new Pen(Color.White, 4), 8, 16, 24, 16);

            using (FileStream fs = new FileStream("icon.ico", FileMode.Create)) {
                // Converting the Bitmap to an Icon and saving it generates a valid .ico file
                Icon.FromHandle(bmp.GetHicon()).Save(fs);
            }
        }
    }
}
"@

Write-Host "Generating Application Icon (icon.ico)..." -ForegroundColor Yellow
Add-Type -TypeDefinition $iconGenCode -ReferencedAssemblies System.Drawing
[IconGenerator]::CreateIco()

# 2. Compile the application
Write-Host "Publishing standalone executable..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true --self-contained false

Write-Host "`nBuild Complete!" -ForegroundColor Green
Write-Host "Your executable is located at:" -ForegroundColor White
Write-Host "$PWD\bin\Release\net8.0-windows\win-x64\publish\WindowMinimizerTray.exe" -ForegroundColor Green
