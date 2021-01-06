// Klass: 18Naa2
// Namn: Lucas Ilstedt
// Kurs: Programmering1
// Datum: 02/12-2020.
// U1, U4, U7 färdig under lektionstid 24/11.
    // U1 har fått lite fler alternativ.
    // U7 är däremot inte helt efter uppgiften utan modifierad så den följer standarden för mängd skepp och storlek.
// U2, U5, U6, U8 färdig efter lektionstid 24/11.
// U3 färdig 02/12-2020, resursdag. Tredje omskrivningen av hela metoden lyckades.
// Hela koden innehåller nu kommentarer inuti metoderna också.
// Kända buggar och ofullständiga delar: 
    // 1. Ibland kraschar spelet när den försöker sätta ut ett skep åt datorn som går utanför index.
        // 1.1. Det konstiga är att en if sats några rader ovanför bör fånga det problemet.
            // 1.1.1. Kan vara liknande problem som 4.2.2.
        // 1.2. Buggen dyker upp ytterst sällan vilket gör den väldig svår att felsöka.
    // 2. Buggen fixad i V2. Se tidigare version.
    // 3. Buggen fixad i V2. Se tidigare version.
    // 4. U3 är nu skriven och buggfri i V3. 
        // 4.1. Buggen fixad i V3. Se tidigare version.
        // 4.2. Buggen fixad i V3. Se tidigare version.
        // 4.3. Buggen fixad i V3. Se tidigare version.
    // 5. Spelaren vinner omedelbart alla matcher efter sin första vinst. Buggen fixad i V4.
        // 5.1. Hade att göra med att liven inte återställdes efter matcherna.

using System;
using System.Threading;

namespace Sänka_Skepp
{
    class Program
    {
        static int kartBredd = 10;
        static int kartHöjd = 10;
        static int antalEttSkepp = 0;
        static int antalTvåSkepp = 1;
        static int antalTreSkepp = 2;
        static int antalFyraSkepp = 1;
        static int antalFemSkepp = 1;
        static int spelarensLiv = antalEttSkepp * 1 + antalTvåSkepp * 2 + antalTreSkepp * 3 + antalFyraSkepp * 4 + antalFemSkepp * 5;
        static int datornsLiv = spelarensLiv;
        static int xKoordinat;
        static int yKoordinat;
        static bool test = false;
        static bool smartAI = false;
        static string senasteVinnaren = "Ingen har vunnit ännu";
        static string alfabetet = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ";
        static string[,] spelarensSpelplan = new string[kartBredd, kartHöjd];
        static string[,] datornsSpelplan = new string[kartBredd, kartHöjd];
        static Random slump = new Random();
        static int riktningSlump;

        // Används för att hålla koll på träffarna så spelaren och datorn inte kan skjuta på en position de redan 
        // skjutit på.
        // False för inte skjutit.
        // spelarensSkott håller koll på var spelarens har skjutit och densamma för datornsSkott.
        static bool[,] spelarensSkott = new bool[kartBredd, kartHöjd];
        static bool[,] datornsSkott = new bool[kartBredd, kartHöjd];



        /// <summary>
        /// Kör programmet.
        /// </summary>
        /// <param name="args">Ingen aning.</param>
        static void Main(string[] args)
        {
            // Anropar menymetoden som i sin tur anropar ett antal mindre metoder.
            // Se relevant metod.
            Meny();
        }



        /// <summary>
        /// Bygger spelplanerna och bool[].
        /// </summary>
        static void GenereraSpelplaner()
        {
            // Fyller spelplanerna med tomma rutor.
            // Fyller bool[] med falsk vilket indikerar att positionen inte har skjutits på ännu.
            for (int i = 0; i < kartBredd; i++)
            {
                for (int j = 0; j < kartHöjd; j++)
                {
                    spelarensSpelplan[i, j] = "O";
                    datornsSpelplan[i, j] = "O";
                    spelarensSkott[i, j] = false;
                    datornsSkott[i, j] = false;
                }
            }
        }



        /// <summary>
        /// Placerar ut skeppen åt spelaren och datorn.
        /// </summary>
        static void PlaceraUtSkeppen()
        {
            // Tillåt spelaren att själv lägga ut sina skepp.
            // Upprepar för varje skeppstorlek och antal av den storleken.
            // Se relevant metod.
            for (int i = 0; i < antalEttSkepp; i++)
            {
                Placering(1, i);
            }
            for (int i = 0; i < antalTvåSkepp; i++)
            {
                Placering(2, i);
            }
            for (int i = 0; i < antalTreSkepp; i++)
            {
                Placering(3, i);
            }
            for (int i = 0; i < antalFyraSkepp; i++)
            {
                Placering(4, i);
            }
            for (int i = 0; i < antalFemSkepp; i++)
            {
                Placering(5, i);
            }

            // Genererar datorns skepp, se relevanta metoder.
            // Är mer eller mindre densamma, enda som skiljer de åt är hur stora skeppen är.
            // Det är i några av de som bugg 1. uppstår.
            // Får felsöka det i senare version.

            // Se GenereraTvåSkeppen() för kommentarer i metoden. 
            // Alla andra är mer eller mindre densamma och jag undviker onödig upprepning så här.
            GenereraFemSkeppen();
            GenereraFyraSkeppen();
            GenereraTreSkeppen();
            GenereraTvåSkeppen();
            GenereraEttSkeppen();
        }



        /// <summary>
        /// Kör spelet.
        /// </summary>
        static void SpelaSpelet()
        {
            // Skapar spelplanerna.
            GenereraSpelplaner();
            
            // Placerar ut skeppen.
            PlaceraUtSkeppen();

            // Återställer liven mellan matcher, pekades ut av Per i V3.
            spelarensLiv = antalEttSkepp * 1 + antalTvåSkepp * 2 + antalTreSkepp * 3 + antalFyraSkepp * 4 + antalFemSkepp * 5;
            datornsLiv = spelarensLiv;

            // Fortsätter till spelaren eller datorn har slut på skepp.
            // Varje ruta på skeppen representerar ett liv.
            // Ett utvecklingsförslag på progsharp var att göra så det kan bli oavgjort.
            // Detta skulle kunna fixas i DatornsSkott() men jag ville inte ha det.
            // Blir väl ganska svårt att skjuta ett sista skott när man redan har sjunkit?
            while (spelarensLiv > 0 && datornsLiv > 0)
            {
                // Rensar efter varje omgång.
                Console.Clear();
                RitaSpelplanerna();
                SpelarensSkott();
                DatornsSkott();
            }

            // Anger vinnaren.
            if (spelarensLiv == 0)
            {
                Console.WriteLine("Datorn vann!");
                senasteVinnaren = "Datorn";
            }
            else
            {
                Console.WriteLine("Du vann!");
                // Tillåter en att skriva in sitt namn vilket kan visas i huvudmenyn.
                Console.WriteLine("Skriv in ditt namn");
                senasteVinnaren = Console.ReadLine();
            }
        }



        /// <summary>
        /// Tillåter användaren att ändra inställningarna i spelet.
        /// </summary>
        static void SpelInställningar()
        {
            int menyVal = 0;

            // Det var tänk att fylla den här metoden med en massa inställningar.
            // Detta kanske blir aktuellt i en eventuell V4 men fram t.o.m V3 så har prioriteringen legat på uppgiften som angiven i classroom.

            // Ändra storlek på spelplanen.
            // Slå på smartare AI.
            // När av så skjuter datorn helt random.
            // När på så skjuter datorn nära tidigare träffar.
            // Kan ha roliga inställningar också, t.ex. ändra färgen på träffar, missar, spelplan, bakgrund osv.
            // Traditionella koordinater, t.ex. "A7".
            // Skapa egen metod för denna som även ser till att man inte kan skriva fel.

            while (menyVal != 2)
            {
                Console.Clear();
                Console.WriteLine("För att slå på/av Smart AI, skriv \"1\"");
                Console.WriteLine("För att gå tillbaka till huvudmenyn, skriv \"2\"");
                menyVal = ReadInt();
                switch (menyVal)
                {
                    case 1:
                        if (!smartAI)
                        {
                            // Sätter på smartAI vilket innebär att datorn skjuter på rutor bredvid tidigare träffar.
                            smartAI = true;
                            Console.WriteLine("Smart AI är nu påslaget.");
                        }
                        else
                        {
                            // Slår av smartAI
                            smartAI = false;
                            Console.WriteLine("Smart AI är nu avslaget.");
                        }
                        Thread.Sleep(2000);
                        break;

                    case 2:
                        break;

                    default:
                        Console.WriteLine("Du angav inte ett korrekt alternativ.");
                        Thread.Sleep(2000);
                        break;
                }
            }
        }



        /// <summary>
        /// Skriver ut den senaste vinnaren.
        /// </summary>
        static void SenasteVinnaren()
        {
            // Skriver ut standardtexten om ingen har vunnit sedan programmet startades.
            // Hade vi gjort utvecklingsförslaget där man kunde spara inställningar,
            // så hade den här delen bara behövts till en vinnare kröns första gången någonsin, 
            // sen skulle den komma ihåg det mellan avstängningar i programmet.
            if (senasteVinnaren == "Ingen har vunnit ännu")
            {
                Console.WriteLine(senasteVinnaren);
            }
            else
            {
                Console.WriteLine($"Senaste vinnaren: {senasteVinnaren}");
            }
            Console.ReadKey();
        }



        /// <summary>
        /// Skriver ut spelreglerna.
        /// </summary>
        static void SpelRegler()
        {
            // Menat att skriva ut spelreglerna, just nu har den bara en placeholder, fixas i V4 om det blir en.
            Console.WriteLine("Spelregler");
            Console.ReadKey();
        }



        /// <summary>
        /// int.TryParse metod som låter en fortsätta ange textsträngar till man anger ett heltal.
        /// Kopierad från kapitel 6.
        /// </summary>
        /// <returns>Slutgiltiga heltalet</returns>
        static int ReadInt()
        {
            // Används som exempel i progsharp, rekommenderas ist för int.Parse().
            // Tyvärr är jag fortfarande inte helt säker på hur TryParse fungerar,
            // och kan därmed inte skriva relevanta kommentarer.

            int heltal;
            while (int.TryParse(Console.ReadLine(), out heltal) == false)
            {
                Console.WriteLine("Du skrev inte in ett heltal. Försök igen.");
            }
            return heltal;
        }



        /// <summary>
        /// Skriver ut menytexten.
        /// </summary>
        static void MenyText()
        {
            Console.WriteLine("Välkommen till Sänka Skepp!");
            Console.WriteLine("För att spela spelet, skriv \"1\"");
            Console.WriteLine("För att se senaste vinnaren, skriv \"2\"");
            Console.WriteLine("För att få spelet förklarat, skriv \"3\"");
            Console.WriteLine("För att ändra spelinställningarna, skriv \"4\"");
            Console.WriteLine("För att aktivera \"Test\", skriv \"5\"");
            Console.WriteLine("För att avsluta programmet, skriv \"6\"");
        }



        /// <summary>
        /// Ritar ut spelplanerna varje runda.
        /// </summary>
        static void RitaSpelplanerna()
        {
            // Skriver ut spelarens spelplan.
            for (int i = 0; i < kartHöjd; i++)
            {
                if (i == 0)
                {
                    Console.Write("   ");
                    for (int j = 0; j < kartBredd; j++)
                    {
                        Console.Write(alfabetet[j]);
                    }
                    for (int j = 0; j < 5; j++)
                    {
                        Console.Write(" ");
                    }
                    for (int j = 0; j < kartBredd; j++)
                    {
                        Console.Write(alfabetet[j]);
                    }
                    Console.WriteLine();
                }

                Console.Write(i + 1);
                if (i != 9)
                {
                    Console.Write("  ");
                }
                else
                {
                    Console.Write(" ");
                }

                for (int j = 0; j < kartBredd; j++)
                {
                    // Målar ut skeppen i en annan färg än de tomma rutorna.
                    if (spelarensSpelplan[j, i] == "X")
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }
                    // Ändrar textfärgen till röd om datorn har skjutit på rutan.
                    if (datornsSkott[j, i] == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(spelarensSpelplan[j, i]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write("     ");

                // Skriver ut datorns spelplan.
                for (int j = 0; j < kartBredd; j++)
                {
                    // Ändrar textfärgen till röd om spelaren har skjutit på rutan.
                    // Och skriver ut vad som låg på rutan.
                    if (spelarensSkott[j, i] == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(datornsSpelplan[j, i]);
                    }
                    else if (test)
                    {
                        // Skriver ut alla rutor om testläget är på.
                        Console.Write(datornsSpelplan[j, i]);
                    }
                    else
                    {
                        Console.Write("-");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            
            // Anger hur många liv spelaren och datorn har kvar.
            Console.WriteLine($"Du har {spelarensLiv} liv kvar.");
            Console.WriteLine($"Datorn har {datornsLiv} liv kvar.");
            Console.WriteLine();
        }



        /// <summary>
        /// Låter spelaren skjuta på en position.
        /// </summary>
        static void SpelarensSkott()
        {
            // Se relevant metod.
            // Mest bara för att undvika onödig repitition.
            SpelarSkott();

            // Ser till att spelaren inte kan skjuta utanför spelplanen.
            if (xKoordinat >= kartBredd || xKoordinat < 0 || yKoordinat >= kartHöjd || yKoordinat < 0)
            {
                while (xKoordinat >= kartBredd || xKoordinat < 0 || yKoordinat >= kartHöjd || yKoordinat < 0)
                {
                    Console.WriteLine("Positionen är utanför spelplanen, var god ange en annan.");
                    SpelarSkott();
                }
            }

            // Ser till att spelaren inte kan skjuta på en position de redan skjutit på.
            if (spelarensSkott[xKoordinat, yKoordinat] == true)
            {
                while (spelarensSkott[xKoordinat, yKoordinat] == true)
                {
                    Console.WriteLine("Du har redan skjutit där, var god ange en annan position.");
                    SpelarSkott();
                }
            }

            // Anger träff eller miss.
            if (datornsSpelplan[xKoordinat, yKoordinat] == "X")
            {
                Console.WriteLine("Träff!");
                Thread.Sleep(2000);
                // Håller koll på datorns resterande skepp.
                datornsLiv--;
            }
            else
            {
                Console.WriteLine("Miss");
                Thread.Sleep(2000);
            }
            // Sparar skottets position.
            spelarensSkott[xKoordinat, yKoordinat] = true;
        }



        /// <summary>
        /// Slumpar ett skott åt datorn.
        /// </summary>
        static void DatornsSkott()
        {
            bool skjutitDenHärRundan = false;
            bool kollatUpp = false;
            bool kollatNer = false;
            bool kollatHöger = false;
            bool kollatVänster = false;

            // Ser till att datorn inte får köra om senaste skottet sänkte sista skeppet.
            // Det är denna del som kan ändras om man vill ha med oavgjort.
            // Vanligt läge.
            if (datornsLiv > 0 && !smartAI)
            {
                // Slumpar en position för datorn att skjuta på.
                xKoordinat = slump.Next(kartBredd);
                yKoordinat = slump.Next(kartHöjd);

                // Slumpar en ny position om datorn redan har skjutit på den slumpade positionen.
                if (datornsSkott[xKoordinat, yKoordinat] == true)
                {
                    while (datornsSkott[xKoordinat, yKoordinat] == true)
                    {
                        xKoordinat = slump.Next(kartBredd);
                        yKoordinat = slump.Next(kartHöjd);
                    }
                }

                // Anger träff eller miss.
                // Än en gång mest bara för att undvika onödig repitition såsom SpelarSkott().
                DatornTräffadeEllerInte(xKoordinat, yKoordinat);

                // Håller koll på var datorn har skjutit.
                datornsSkott[xKoordinat, yKoordinat] = true;
            }

            // SmartAI läge.
            if (datornsLiv > 0 && smartAI)
            {
                // Söker efter tidigare träffar.
                for (int i = 0; i < kartBredd; i++)
                {
                    // Ser till att datorn bara skjuter en gång per runda.
                    // Annars skulle den skjuta bredvid varje träff på en runda,
                    // och alltid döda spelaren rundan efter sin första träff.
                    if (skjutitDenHärRundan)
                    {
                        break;
                    }
                    for (int j = 0; j < kartHöjd; j++)
                    {
                        // Håller koll på vilka riktningar den har kollat varje runda.
                        // Annars skulle den fortsätta kolla bredvid en träff hela tiden.
                        // Lite dåligt förklarat men det reglerar en while-loop längre ner.
                        kollatHöger = false;
                        kollatNer = false;
                        kollatUpp = false;
                        kollatVänster = false;

                        // Ser till att datorn bara skjuter en gång per runda.
                        if (skjutitDenHärRundan)
                        {
                            break;
                        }

                        // Kollar bredvid en tidigare träff.
                        if (spelarensSpelplan[i, j] == "X" && datornsSkott[i, j] && !skjutitDenHärRundan)
                        {
                            // Kollar varje ruta bredvid träffen till den har skjutit igen.
                            while (!skjutitDenHärRundan && (!kollatHöger || !kollatNer || !kollatUpp || !kollatVänster))
                            {
                                riktningSlump = slump.Next(4);

                                switch (riktningSlump)
                                {
                                    // Skjuter vänster.
                                    case 0: //V
                                        // Anger att den har kollat rutan till vänster om den tidigare träffen.
                                        kollatVänster = true;
                                        if (i != 0)
                                        {
                                            // Kollar om den redan har skjutit på den rutan.
                                            if (!datornsSkott[i - 1, j])
                                            {
                                                // Anger att den skjuter på den rutan.
                                                datornsSkott[i - 1, j] = true;
                                                
                                                // Anger att den har skjutit den här rundan.
                                                skjutitDenHärRundan = true;

                                                // Anger var den skjuter och om den träffar eller inte.
                                                DatornTräffadeEllerInte(i - 1, j);
                                            }
                                        }
                                        break;
                                    
                                    // Skjuter höger.
                                    case 1: //H
                                        kollatHöger = true;
                                        if (i != kartHöjd)
                                        {
                                            if (!datornsSkott[i + 1, j])
                                            {
                                                datornsSkott[i + 1, j] = true;
                                                skjutitDenHärRundan = true;
                                                DatornTräffadeEllerInte(i + 1, j);
                                            }
                                        }
                                        break;
                                
                                    // Skjuter upp.
                                    case 2: //U
                                        kollatUpp = true;
                                        if (j != 0)
                                        {
                                            if (!datornsSkott[i, j - 1])
                                            {
                                                datornsSkott[i, j - 1] = true;
                                                skjutitDenHärRundan = true;
                                                DatornTräffadeEllerInte(i, j - 1);
                                            }
                                        }
                                        break;

                                    // Skjuter ned.
                                    case 3: //N
                                        kollatNer = true;
                                        if (j != kartBredd)
                                        {
                                            if (!datornsSkott[i, j + 1])
                                            {
                                                datornsSkott[i, j + 1] = true;
                                                skjutitDenHärRundan = true;
                                                DatornTräffadeEllerInte(i, j + 1);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }

                // Skjuter på en random position om den redan har skjutit på alla rutor bredvid alla träffar.
                if (!skjutitDenHärRundan)
                {
                    // Slumpar en position för datorn att skjuta på.
                    xKoordinat = slump.Next(kartBredd);
                    yKoordinat = slump.Next(kartHöjd);

                    // Slumpar en ny position om datorn redan har skjutit på den slumpade positionen.
                    if (datornsSkott[xKoordinat, yKoordinat] == true)
                    {
                        while (datornsSkott[xKoordinat, yKoordinat] == true)
                        {
                            xKoordinat = slump.Next(kartBredd);
                            yKoordinat = slump.Next(kartHöjd);
                        }
                    }

                    // Anger träff eller miss.
                    DatornTräffadeEllerInte(xKoordinat, yKoordinat);

                    // Håller koll på var datorn har skjutit.
                    datornsSkott[xKoordinat, yKoordinat] = true;
                }
            }
        }



        /// <summary>
        /// Genererar datorns EnRutiga skepp.
        /// </summary>
        static void GenereraEttSkeppen()
        {
            for (int i = 0; i < antalEttSkepp; i++)
            {
                xKoordinat = slump.Next(kartBredd);
                yKoordinat = slump.Next(kartHöjd);

                if (datornsSpelplan[xKoordinat, yKoordinat] == "X")
                {
                    while (datornsSpelplan[xKoordinat, yKoordinat] == "X")
                    {
                        xKoordinat = slump.Next(kartBredd);
                        yKoordinat = slump.Next(kartHöjd);
                    }
                }

                if (datornsSpelplan[xKoordinat, yKoordinat] != "X")
                {
                    datornsSpelplan[xKoordinat, yKoordinat] = "X";
                }
            }
        }



        /// <summary>
        /// Genererar datorns TvåRutiga skepp.
        /// </summary>
        static void GenereraTvåSkeppen()
        {
            // Upprepar placeringen för varje antal av skeppen.
            for (int i = 0; i < antalTvåSkepp; i++)
            {
                // Slumpar en position på brädet och en riktning att fortsätta skeppet åt.
                xKoordinat = slump.Next(kartBredd);
                yKoordinat = slump.Next(kartHöjd);
                riktningSlump = slump.Next(4);

                // Kollar ifall det redan ligger ett skepp på positionen sedan tidigare.
                if (datornsSpelplan[xKoordinat, yKoordinat] == "X")
                {
                    // Slumpar en ny position till den har hittat en tom.
                    while (datornsSpelplan[xKoordinat, yKoordinat] == "X")
                    {
                        xKoordinat = slump.Next(kartBredd);
                        yKoordinat = slump.Next(kartHöjd);
                    }
                }

                switch (riktningSlump)
                {
                    // Upp.
                    case 0:
                        // Ser till att skeppet inte kan fortsätta utanför spelplanen.
                        if (yKoordinat == 0)
                        {
                            // Fortsätter till skeppet kan placeras.
                            while (yKoordinat == 0 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        // Kollar om resterande rutor i riktningen har ett skepp eller inte.
                        // Kollar såklart bara längden av skeppet.
                        if (datornsSpelplan[xKoordinat, yKoordinat - 1] == "X")
                        {
                            // Fortstätter slumpa till det fungerar.
                            while (datornsSpelplan[xKoordinat, yKoordinat - 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }

                        // Placerar ut de relevanta fortsatta delarna.
                        datornsSpelplan[xKoordinat, yKoordinat - 1] = "X";
                        break;

                    // Ned.
                    case 1:
                        if (yKoordinat == kartHöjd - 1)
                        {
                            while (yKoordinat == kartHöjd - 1 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat, yKoordinat + 1] == "X")
                        {
                            // Hur lyckas den få IndexOutOfBounds nedanför? Borde inte if satsen ovan fixa det?
                            while (datornsSpelplan[xKoordinat, yKoordinat + 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat, yKoordinat + 1] = "X";
                        break;

                    // Höger.
                    case 2:
                        if (xKoordinat == kartBredd - 1)
                        {
                            while (xKoordinat == kartBredd - 1 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat + 1, yKoordinat] == "X")
                        {
                            while (datornsSpelplan[xKoordinat + 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat + 1, yKoordinat] = "X";
                        break;

                    // Vänster.
                    case 3:
                        if (xKoordinat == 0)
                        {
                            while (xKoordinat == 0)
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat - 1, yKoordinat] == "X")
                        {
                            while (datornsSpelplan[xKoordinat - 1, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat - 1, yKoordinat] = "X";
                        break;
                }

                // Placerar ut den första rutan.
                // Måste ligga i slutet eftersom en ny position kan slumpas ovanför i riktningarna.
                // Undrar om det är här den okända buggen ligger?
                // Kanske kan vara så att rutan är otillgänglig efter att positionen har slumpats i riktningarna.
                datornsSpelplan[xKoordinat, yKoordinat] = "X";
            }
        }



        /// <summary>
        /// Genererar datorns TreRutiga skepp.
        /// </summary>
        static void GenereraTreSkeppen()
        {
            for (int i = 0; i < antalTreSkepp; i++)
            {
                xKoordinat = slump.Next(kartBredd);
                yKoordinat = slump.Next(kartHöjd);
                riktningSlump = slump.Next(4);

                if (datornsSpelplan[xKoordinat, yKoordinat] == "X")
                {
                    while (datornsSpelplan[xKoordinat, yKoordinat] == "X")
                    {
                        xKoordinat = slump.Next(kartBredd);
                        yKoordinat = slump.Next(kartHöjd);
                    }
                }

                switch (riktningSlump)
                {
                    // Upp.
                    case 0:
                        if (yKoordinat <= 1)
                        {
                            while (yKoordinat <= 1 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat, yKoordinat - 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 2] == "X")
                        {
                            while (datornsSpelplan[xKoordinat, yKoordinat - 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 2] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat, yKoordinat - 1] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat - 2] = "X";
                        break;

                    // Ned.
                    case 1:
                        if (yKoordinat >= kartHöjd - 2)
                        {
                            while (yKoordinat >= kartHöjd - 2 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat, yKoordinat + 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 2] == "X")
                        {
                            while (datornsSpelplan[xKoordinat, yKoordinat + 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 2] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat, yKoordinat + 1] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat + 2] = "X";
                        break;

                    // Höger.
                    case 2:
                        if (xKoordinat >= kartBredd - 2)
                        {
                            while (xKoordinat >= kartBredd - 2 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat + 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 2, yKoordinat] == "X")
                        {
                            while (datornsSpelplan[xKoordinat + 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 2, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat + 1, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat + 2, yKoordinat] = "X";
                        break;

                    // Vänster.
                    case 3:
                        if (xKoordinat <= 1)
                        {
                            while (xKoordinat <= 1)
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat - 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 2, yKoordinat] == "X")
                        {
                            while (datornsSpelplan[xKoordinat - 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 2, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat - 1, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat - 2, yKoordinat] = "X";
                        break;
                }

                datornsSpelplan[xKoordinat, yKoordinat] = "X";
            }
        }



        /// <summary>
        /// Genererar datorns FyrRutiga skepp.
        /// </summary>
        static void GenereraFyraSkeppen()
        {
            for (int i = 0; i < antalFyraSkepp; i++)
            {
                xKoordinat = slump.Next(kartBredd);
                yKoordinat = slump.Next(kartHöjd);
                riktningSlump = slump.Next(4);

                if (datornsSpelplan[xKoordinat, yKoordinat] == "X")
                {
                    while (datornsSpelplan[xKoordinat, yKoordinat] == "X")
                    {
                        xKoordinat = slump.Next(kartBredd);
                        yKoordinat = slump.Next(kartHöjd);
                    }
                }

                switch (riktningSlump)
                {
                    // Upp.
                    case 0:
                        if (yKoordinat <= 2)
                        {
                            while (yKoordinat <= 2 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat, yKoordinat - 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 2] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 3] == "X")
                        {
                            while (datornsSpelplan[xKoordinat, yKoordinat - 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 2] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 3] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat, yKoordinat - 1] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat - 2] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat - 3] = "X";
                        break;

                    // Ned.
                    case 1:
                        if (yKoordinat >= kartHöjd - 3)
                        {
                            while (yKoordinat >= kartHöjd - 3 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat, yKoordinat + 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 2] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 3] == "X")
                        {
                            while (datornsSpelplan[xKoordinat, yKoordinat + 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 2] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 3] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat, yKoordinat + 1] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat + 2] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat + 3] = "X";
                        break;

                    // Höger.
                    case 2:
                        if (xKoordinat >= kartBredd - 3)
                        {
                            while (xKoordinat >= kartBredd - 3 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat + 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 2, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 3, yKoordinat] == "X")
                        {
                            while (datornsSpelplan[xKoordinat + 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 2, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 3, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat + 1, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat + 2, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat + 3, yKoordinat] = "X";
                        break;

                    // Vänster.
                    case 3:
                        if (xKoordinat <= 2)
                        {
                            while (xKoordinat <= 2)
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat - 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 2, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 3, yKoordinat] == "X")
                        {
                            while (datornsSpelplan[xKoordinat - 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 2, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 3, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat - 1, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat - 2, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat - 3, yKoordinat] = "X";
                        break;
                }

                datornsSpelplan[xKoordinat, yKoordinat] = "X";
            }
        }



        /// <summary>
        /// Genererar datorns FemRutiga skepp.
        /// </summary>
        static void GenereraFemSkeppen()
        {
            for (int i = 0; i < antalFemSkepp; i++)
            {
                xKoordinat = slump.Next(kartBredd);
                yKoordinat = slump.Next(kartHöjd);
                riktningSlump = slump.Next(4);

                if (datornsSpelplan[xKoordinat, yKoordinat] == "X")
                {
                    while (datornsSpelplan[xKoordinat, yKoordinat] == "X")
                    {
                        xKoordinat = slump.Next(kartBredd);
                        yKoordinat = slump.Next(kartHöjd);
                    }
                }

                switch (riktningSlump)
                {
                    // Upp.
                    case 0:
                        if (yKoordinat <= 3)
                        {
                            while (yKoordinat <= 3 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat, yKoordinat - 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 2] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 3] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 4] == "X")
                        {
                            while (datornsSpelplan[xKoordinat, yKoordinat - 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 2] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 3] == "X" || datornsSpelplan[xKoordinat, yKoordinat - 4] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat, yKoordinat - 1] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat - 2] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat - 3] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat - 4] = "X";
                        break;

                    // Ned.
                    case 1:
                        if (yKoordinat >= kartHöjd - 4)
                        {
                            while (yKoordinat >= kartHöjd - 4 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat, yKoordinat + 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 2] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 3] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 4] == "X")
                        {
                            while (datornsSpelplan[xKoordinat, yKoordinat + 1] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 2] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 3] == "X" || datornsSpelplan[xKoordinat, yKoordinat + 4] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat, yKoordinat + 1] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat + 2] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat + 3] = "X";
                        datornsSpelplan[xKoordinat, yKoordinat + 4] = "X";
                        break;

                    // Höger.
                    case 2:
                        if (xKoordinat >= kartBredd - 4)
                        {
                            while (xKoordinat >= kartBredd - 4 || datornsSpelplan[xKoordinat, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat + 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 2, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 3, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 4, yKoordinat] == "X")
                        {
                            while (datornsSpelplan[xKoordinat + 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 2, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 3, yKoordinat] == "X" || datornsSpelplan[xKoordinat + 4, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat + 1, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat + 2, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat + 3, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat + 4, yKoordinat] = "X";
                        break;

                    // Vänster.
                    case 3:
                        if (xKoordinat <= 3)
                        {
                            while (xKoordinat <= 3)
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        if (datornsSpelplan[xKoordinat - 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 2, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 3, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 4, yKoordinat] == "X")
                        {
                            while (datornsSpelplan[xKoordinat - 1, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 2, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 3, yKoordinat] == "X" || datornsSpelplan[xKoordinat - 4, yKoordinat] == "X")
                            {
                                xKoordinat = slump.Next(kartBredd);
                                yKoordinat = slump.Next(kartHöjd);
                            }
                        }
                        datornsSpelplan[xKoordinat - 1, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat - 2, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat - 3, yKoordinat] = "X";
                        datornsSpelplan[xKoordinat - 4, yKoordinat] = "X";
                        break;
                }

                datornsSpelplan[xKoordinat, yKoordinat] = "X";
            }
        }



        /// <summary>
        /// Konverterar bokstaven från positionen till rätt int.
        /// </summary>
        /// <param name="bokstav">Bokstaven som spelaren angett</param>
        /// <returns>Int som representerar bokstavens räknenummer i alfabetet</returns>
        static int KonverteraX(char bokstav)
        {
            // Konverterar bokstav till relevant int.
            // Är ju ganska enkelt att göra tvärtom, bara ta int:ens position i alfabetssträngen för att få relevant bokstav.
            // Vet inte om det finns ett lika lätt sätt att göra det här däremot.
            
            int xKoordinat = 0;
            switch (bokstav)
            {
                case 'A':
                    xKoordinat = 1;
                    break;

                case 'B':
                    xKoordinat = 2;
                    break;

                case 'C':
                    xKoordinat = 3;
                    break;

                case 'D':
                    xKoordinat = 4;
                    break;

                case 'E':
                    xKoordinat = 5;
                    break;

                case 'F':
                    xKoordinat = 6;
                    break;

                case 'G':
                    xKoordinat = 7;
                    break;

                case 'H':
                    xKoordinat = 8;
                    break;

                case 'I':
                    xKoordinat = 9;
                    break;

                case 'J':
                    xKoordinat = 10;
                    break;

                case 'K':
                    xKoordinat = 11;
                    break;

                case 'L':
                    xKoordinat = 12;
                    break;

                case 'M':
                    xKoordinat = 13;
                    break;

                case 'N':
                    xKoordinat = 14;
                    break;

                case 'O':
                    xKoordinat = 15;
                    break;

                case 'P':
                    xKoordinat = 16;
                    break;

                case 'Q':
                    xKoordinat = 17;
                    break;

                case 'R':
                    xKoordinat = 18;
                    break;

                case 'S':
                    xKoordinat = 19;
                    break;

                case 'T':
                    xKoordinat = 20;
                    break;

                case 'U':
                    xKoordinat = 21;
                    break;

                case 'V':
                    xKoordinat = 22;
                    break;

                case 'W':
                    xKoordinat = 23;
                    break;

                case 'X':
                    xKoordinat = 24;
                    break;

                case 'Y':
                    xKoordinat = 25;
                    break;

                case 'Z':
                    xKoordinat = 26;
                    break;

                case 'Å':
                    xKoordinat = 27;
                    break;

                case 'Ä':
                    xKoordinat = 28;
                    break;

                case 'Ö':
                    xKoordinat = 29;
                    break;
            }
            return xKoordinat;
        }



        /// <summary>
        /// Säger om datorn träffade eller inte.
        /// </summary>
        static void DatornTräffadeEllerInte(int x, int y)
        {
            char bokstav;

            bokstav = alfabetet[x];

            // Berättar för spelaren var datorn skjuter.
            Console.WriteLine($"Datorn skjuter på {bokstav}{y + 1}");
            Thread.Sleep(500);

            if (spelarensSpelplan[x, y] == "X")
            {
                Console.WriteLine("Datorn träffade!");
                Thread.Sleep(2000);
                // Håller koll på spelarens resterande skepp.
                spelarensLiv--;
            }
            else
            {
                Console.WriteLine("Datorn missade");
                Thread.Sleep(2000);
            }
        }



        /// <summary>
        /// Menyprogram.
        /// </summary>
        static void Meny()
        {
            // Se relevanta metoder.

            int menyVal = 0;

            // while menyprogram.
            while (menyVal != 6)
            {
                Console.Clear();

                MenyText();
                menyVal = ReadInt();

                Console.Clear();

                switch (menyVal)
                {
                    case 1:
                        SpelaSpelet();
                        break;

                    case 2:
                        SenasteVinnaren();
                        break;

                    case 3:
                        SpelRegler();
                        break;

                    case 4:
                        SpelInställningar();
                        break;

                    case 5:
                        test = true;
                        break;

                    // Avslutar programmet.
                    case 6:
                        break;

                    default:
                        Console.WriteLine("Du angav inte ett korrekt alternativ, du flyttas till huvudmenyn.");
                        Thread.Sleep(2000);
                        break;
                }
            }
        }



        /// <summary>
        /// Placerar ut spelarens skepp.
        /// </summary>
        /// <param name="storlekPåSkepp">Antal rutor som skeppet innefattar</param>
        /// <param name="räknare">variabel i for loopen</param>
        static void Placering(int storlekPåSkepp, int räknare)
        {
            string position;
            char bokstav;
            string text;
            bool skeppPlacerat = false;
            bool kanPlaceras = true;
            int gammalX;
            int gammalY;

            // Tillåter spelaren att fortsätta leta för ett ställe att placera sitt skepp till den har lyckats med det.
            while (!skeppPlacerat)
            {
                // Återställer booleans för att de kan har påverkats under placeringen.
                // Innan denna återställning var med fanns en bugg som gjorde det så att man inte kunde placera sitt skepp,
                // om man inte gjorde det på första körningen av while-loopen.
                // Märkte det ganska snabbt däremot och hade en aning om vad som orsakade det.
                skeppPlacerat = false;
                kanPlaceras = true;
                
                // Skriver ut spelarens karta så man kan se var tidigare skepp ligger.
                Console.Clear();
                Console.Write("   ");
                for (int j = 0; j < kartBredd; j++)
                {
                    Console.Write(alfabetet[j]);
                }
                Console.WriteLine();

                for (int i = 0; i < kartBredd; i++)
                {
                    Console.Write(i + 1);
                    if (i < 9)
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                    for (int j = 0; j < kartHöjd; j++)
                    {
                        // "Highlightar" tidigare skepp så de står ut.
                        if (spelarensSpelplan[j, i] == "X")
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Console.Write(spelarensSpelplan[j, i]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();

                // Läser in var man vill placera sitt skepp.
                Console.WriteLine($"Vilken position vill du lägga ena änden av skepp nr {räknare + 1} av storlek {storlekPåSkepp} på? " +
                $"Svara som t.ex. \"A5\"");
                position = Console.ReadLine().Trim().ToUpper();
                // Anpassar värdet till arrayen.
                bokstav = position[0];
                xKoordinat = KonverteraX(bokstav) - 1;
                if (position.Length == 2)
                {
                    text = Convert.ToString(position[1]);
                    yKoordinat = int.Parse(text) - 1;
                }
                else
                {
                    text = Convert.ToString(position[1]);
                    yKoordinat = 10 * int.Parse(text);
                    text = Convert.ToString(position[2]);
                    yKoordinat += int.Parse(text) - 1;
                }

                // Kollar så att positionen inte är utanför spelplanen.
                if (yKoordinat >= kartHöjd || yKoordinat < 0 || xKoordinat >= kartBredd || xKoordinat < 0)
                {
                    Console.WriteLine("Du angav en position som var utanför spelplanen. Skepplaceringen börjar om.");
                    Thread.Sleep(2000);
                    continue;
                }

                // Kollar så att positionen inte redan har ett skepp sen tidigare.
                if (spelarensSpelplan[xKoordinat, yKoordinat] == "X")
                {
                    Console.Write("Du angav en position som redan hade ett skepp. Skepplaceringen börjar om.");
                    Thread.Sleep(2000);
                    continue;
                }

                // Frågar vilken riktning man vill placera sitt skepp.
                // Anger även vilka positioner man ska skriva in för att fortsätta i de riktningar.
                // Ytterliggare skriver den inte ut riktningar som är utanför spelplanen.
                // Kom på nu när jag skriver de här kommentarerna att den kanske inte heller bör skriva ut riktningar som redan har ett skepp.
                // Aja, den förklarar ju det senare.
                Console.Write("Vilken riktning vill du fortsätta skeppet åt? Svara som: ");
                if (xKoordinat == 0 && yKoordinat == 0)
                {
                    // Höger och ner. Övre vänstra hörnet. "A1"
                    Console.WriteLine($"{alfabetet[xKoordinat + 1]}{yKoordinat + 1} eller {alfabetet[xKoordinat]}{yKoordinat + 2}");
                }
                else if (xKoordinat == kartBredd - 1 && yKoordinat == 0)
                {
                    // Vänster och ner. Övre högra hörnet. "J1"
                    Console.WriteLine($"{alfabetet[xKoordinat - 1]}{yKoordinat + 1} eller {alfabetet[xKoordinat]}{yKoordinat + 2}");
                }
                else if (xKoordinat == 0 && yKoordinat == kartHöjd - 1)
                {
                    // Höger och upp. Nedre vänstra hörnet. "A10"
                    Console.WriteLine($"{alfabetet[xKoordinat + 1]}{yKoordinat + 1} eller {alfabetet[xKoordinat]}{yKoordinat}");
                }
                else if (xKoordinat == kartBredd - 1 && yKoordinat == kartHöjd - 1)
                {
                    // Vänster och upp. Nedre högra hörnet. "J10"
                    Console.WriteLine($"{alfabetet[xKoordinat - 1]}{yKoordinat + 1} eller {alfabetet[xKoordinat]}{yKoordinat}");
                }
                else if (yKoordinat == 0)
                {
                    // Vänster, höger och ner. Toppen raden. "Y1"
                    Console.WriteLine($"{alfabetet[xKoordinat - 1]}{yKoordinat + 1}, {alfabetet[xKoordinat + 1]}{yKoordinat + 1} eller {alfabetet[xKoordinat]}{yKoordinat + 2}");
                }
                else if (xKoordinat == 0)
                {
                    // Upp, ner och höger. Vänstraste kolumnen. "AX"
                    Console.WriteLine($"{alfabetet[xKoordinat]}{yKoordinat}, {alfabetet[xKoordinat]}{yKoordinat + 2} eller {alfabetet[xKoordinat + 1]}{yKoordinat + 1}");
                }
                else if (xKoordinat == kartBredd - 1)
                {
                    // Upp, ner och vänster. Högersta kolumnen. "JX"
                    Console.WriteLine($"{alfabetet[xKoordinat]}{yKoordinat}, {alfabetet[xKoordinat]}{yKoordinat + 2} eller {alfabetet[xKoordinat - 1]}{yKoordinat + 1}");
                }
                else if (yKoordinat == kartHöjd - 1)
                {
                    // Upp, vänster och höger. Botten raden. "Y10"
                    Console.WriteLine($"{alfabetet[xKoordinat]}{yKoordinat}, {alfabetet[xKoordinat - 1]}{yKoordinat + 1} eller {alfabetet[xKoordinat + 1]}{yKoordinat + 1}");
                }
                else
                {
                    // Alla riktningar. Resterande positioner.
                    Console.WriteLine($"{alfabetet[xKoordinat]}{yKoordinat}, {alfabetet[xKoordinat]}{yKoordinat + 2}, {alfabetet[xKoordinat - 1]}{yKoordinat + 1} eller {alfabetet[xKoordinat + 1]}{yKoordinat + 1}");
                }

                // Läser in vilken riktning man vill fortsätta i och anpassar till arrayen.
                gammalX = xKoordinat;
                gammalY = yKoordinat;
                position = Console.ReadLine().Trim().ToUpper();
                bokstav = position[0];
                xKoordinat = KonverteraX(bokstav) - 1;
                if (position.Length == 2)
                {
                    text = Convert.ToString(position[1]);
                    yKoordinat = int.Parse(text) - 1;
                }
                else
                {
                    text = Convert.ToString(position[1]);
                    yKoordinat = 10 * int.Parse(text);
                    text = Convert.ToString(position[2]);
                    yKoordinat += int.Parse(text) - 1;
                }
                
                // Kollar så att man inte angav ett värde utanför arrayen.
                // Kanske känns lite onödig eftersom strängarna ovanför inte har angett att man kan skriva i den riktningen.
                // Men om man vill så kan man ju ange en koordinat även om det inte står i alternativen.
                if (yKoordinat >= kartHöjd || yKoordinat < 0 || xKoordinat >= kartBredd || xKoordinat < 0)
                {
                    Console.WriteLine("Du angav en position som var utanför spelplanen. Skepplaceringen börjar om.");
                    Thread.Sleep(2000);
                    continue;
                }

                // Fortsätter åt höger om man har angivit det.
                if (xKoordinat == gammalX + 1 && yKoordinat == gammalY)
                {
                    // Höger.
                    // Kollar ifall någon ruta i fortsatt riktning redan har ett skepp eller är utanför planen.
                    for (int i = 0; i < storlekPåSkepp - 1; i++)
                    {
                        if (xKoordinat >= kartBredd)
                        {
                            Console.WriteLine("Skeppet var för stort för att placeras i den riktningen. Skepplaceringen börjar om.");
                            Thread.Sleep(2000);
                            kanPlaceras = false;
                            break;
                        }
                        if (spelarensSpelplan[xKoordinat, yKoordinat] == "X")
                        {
                            Console.WriteLine($"Ett skepp var redan placerat på {alfabetet[xKoordinat]}{yKoordinat + 1}. Skepplaceringen börjar om.");
                            Thread.Sleep(2000);
                            kanPlaceras = false;
                            break;
                        }
                        // Fortsätter till nästa ruta.
                        xKoordinat++;
                    }

                    // Placerar ut skeppet om inga problem hittades.
                    if (kanPlaceras)
                    {
                        for (int i = gammalX; i < gammalX + storlekPåSkepp; i++)
                        {
                            spelarensSpelplan[i, yKoordinat] = "X";
                        }
                        skeppPlacerat = true;
                    }
                }
                else if (xKoordinat == gammalX - 1 && yKoordinat == gammalY)
                {
                    // Vänster.
                    for (int i = 0; i < storlekPåSkepp - 1; i++)
                    {
                        if (xKoordinat < 0)
                        {
                            Console.WriteLine("Skeppet var för stort för att placeras i den riktningen. Skepplaceringen börjar om.");
                            Thread.Sleep(2000);
                            kanPlaceras = false;
                            break;
                        }
                        if (spelarensSpelplan[xKoordinat, yKoordinat] == "X")
                        {
                            Console.WriteLine($"Ett skepp var redan placerat på {alfabetet[xKoordinat]}{yKoordinat + 1}. Skepplaceringen börjar om.");
                            Thread.Sleep(2000);
                            kanPlaceras = false;
                            break;
                        }
                        xKoordinat--;
                    }

                    if (kanPlaceras)
                    {
                        for (int i = gammalX; i > gammalX - storlekPåSkepp; i--)
                        {
                            spelarensSpelplan[i, yKoordinat] = "X";
                        }
                        skeppPlacerat = true;
                    }
                }
                else if (yKoordinat == gammalY + 1 && xKoordinat == gammalX)
                {
                    // Ner.
                    for (int i = 0; i < storlekPåSkepp - 1; i++)
                    {
                        if (yKoordinat >= kartHöjd)
                        {
                            Console.WriteLine("Skeppet var för stort för att placeras i den riktningen. Skepplaceringen börjar om.");
                            Thread.Sleep(2000);
                            kanPlaceras = false;
                            break;
                        }
                        if (spelarensSpelplan[xKoordinat, yKoordinat] == "X")
                        {
                            Console.WriteLine($"Ett skepp var redan placerat på {alfabetet[xKoordinat]}{yKoordinat + 1}. Skepplaceringen börjar om.");
                            Thread.Sleep(2000);
                            kanPlaceras = false;
                            break;
                        }
                        yKoordinat++;
                    }

                    if (kanPlaceras)
                    {
                        for (int i = gammalY; i < gammalY + storlekPåSkepp; i++)
                        {
                            spelarensSpelplan[xKoordinat, i] = "X";
                        }
                        skeppPlacerat = true;
                    }
                }
                else if (yKoordinat == gammalY - 1 && xKoordinat == gammalX)
                {
                    // Upp.
                    for (int i = 0; i < storlekPåSkepp - 1; i++)
                    {
                        if (yKoordinat < 0)
                        {
                            Console.WriteLine("Skeppet var för stort för att placeras i den riktningen. Skepplaceringen börjar om.");
                            Thread.Sleep(2000);
                            kanPlaceras = false;
                            break;
                        }
                        if (spelarensSpelplan[xKoordinat, yKoordinat] == "X")
                        {
                            Console.WriteLine($"Ett skepp var redan placerat på {alfabetet[xKoordinat]}{yKoordinat + 1}. Skepplaceringen börjar om.");
                            Thread.Sleep(2000);
                            kanPlaceras = false;
                            break;
                        }
                        yKoordinat--;
                    }

                    if (kanPlaceras)
                    {
                        for (int i = gammalY; i > gammalY - storlekPåSkepp; i--)
                        {
                            spelarensSpelplan[xKoordinat, i] = "X";
                        }
                        skeppPlacerat = true;
                    }
                }
                // Startar om placeringen av skeppet om spelaren inte angav en ruta bredvid den första rutan.
                else
                {
                    Console.WriteLine("Du angav inte en korrekt koordinat. Skepplaceringen börjar om.");
                    Thread.Sleep(2000);
                    continue;
                }
            }
        }



        /// <summary>
        /// Används för att läsa in spelarens skott, undviker onödig repitition.
        /// </summary>
        static void SpelarSkott()
        {
            string position;
            char bokstav;
            string text;

            // Läser position för spelarens skott.
            Console.WriteLine("Vilken position vill du skjuta på? Svara som t.ex. \"A5\"");
            // Anpassar värdet till arrayen.
            position = Console.ReadLine().Trim().ToUpper();
            bokstav = position[0];
            xKoordinat = KonverteraX(bokstav) - 1;
            if (position.Length == 2)
            {
                text = Convert.ToString(position[1]);
                yKoordinat = int.Parse(text) - 1;
            }
            else
            {
                text = Convert.ToString(position[1]);
                yKoordinat = 10 * int.Parse(text);
                text = Convert.ToString(position[2]);
                yKoordinat += int.Parse(text) - 1;
            }
        }
    }
}
