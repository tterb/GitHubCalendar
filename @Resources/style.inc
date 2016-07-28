[Variables]

Contribs=0
EmptyColor=155,155,155,55
Color1=239,255,158,200
Color2=130,188,91,250
Color3=80,155,75,250
Color4=10,105,6,250
TextColor=255,255,255,210
TextFont=Serif
TextSize=9
TextGap=20
Radius=6
SquareGap=13
SquareSize=(#Radius# * 2)
TextWidth=((#SquareGap# * 2) + #SquareSize#)
ScraperPath="#@#GitHubScraper/GitHubScraper/bin/Debug/GitHubScraper.exe"
ProfilePath="https://github.com/#USERNAME#"

[BackgroundStyle]
ImageName=#@#images/background.png
ImageTint=0,0,0,80
X=0
Y=0
H=130
W=165

[LogoStyle]
ImageName=#@#images/github_logo.png
ImageTint=235,235,235,125
X=142
Y=108
H=18
W=18

[SquareStyle]
X=r
Y=#SquareGap#r
H=#SquareSize#
W=#SquareSize#
Solid=1
LineStart=0
AntiAlias=1
SolidColor=#EmptyColor#
LineLength=#BlockRadius#
MouseLeaveAction=[!SetOption Active Text " "][!UpdateMeter Active][!Redraw]

[ActiveStyle]
X=85
Y=109
H=#SquareSize#
W=#SquareSize#
FontFace=#TextFont#
FontSize=#TextSize#
FontColor=#TextColor#
StringAlign=CENTER
AntiAlias=1

[WeekStartStyle]
X=#SquareGap#r
Y=15

[LabelStyle]
FontSize=7
FontFace=#TextStyle#
FontColor=#TextColor#
X=3
Y=25r
H=#Radius#
W=#Radius#

[ConfigDesc]
FontFace=#TextFont#
FontSize=14
StringStyle=Bold
FontColor=230,230,230,240
StringEffect=Shadow
FontEffectColor = 0,0,0,200

[ConfigTextField]
FontFace=Monospaced
FontSize=11
StringStyle=Bold
StringEffect=Shadow
SolidColor=80,80,80,185
FontColor=120,120,120, 200
FontEffectColor = 0,0,0,100
AntiAlias=1
