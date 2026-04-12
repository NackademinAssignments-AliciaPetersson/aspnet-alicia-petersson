# aspnet-alicia-petersson

Jag har lyckats bocka av alla G-krav, tror jag. Men hann tyvärr inte så långt som jag hade velat på VG-kraven. Men gjorde så långt jag hann.

##
Jag använde User Secrets för att hantera Client Id och Client Secret vid tredje parts inloggning. Jag har valt att enbart använda GitHub som tredjepartsinloggning. Dom andra ikonerna fungerar inte för tredjeparts-inloggning.

##
Ladda ner projektet och öppna upp det i Visual Studio, ange Presentation.WebApp som startup projekt och starta upp projektet via dev-profilen. (Det är där jag har testat mesta av funktionaliteten, hann inte kolla så mycket att allt fungerade i Prod-profilen. Men jag såg dock att den data som skulle seedas in sparades som det skulle)

Jag har använt en lokal databas-fil för SqlServer som sparas lokalt inne i projektet, men som inte följer med till github. Ändra connection stringen i appsettings.json filen till din egen databas.
