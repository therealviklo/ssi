ssi
===
En interpreterare för stenhögsspråket från Algoritmik av Lennart Salling.

Användning
----------
Programmet körs från kommandoraden, med filerna som ska köras som parametrar. Om flera filer finns som parametrar så läser programmet dem i tur och ordning som om de vore en enda fil.

Man kan använda flaggan `-p` för att få programmet att skriva ut alla steg det gör, men det är inte världens finaste output, så det är mer av en debugfunktionalitet.

Funktioner
----------
Funktioner definieras såhär:
```
Öka x med fem =
	Öka x; Öka x
	Öka x
	Öka x;
	Öka x
```
Det måste vara en nyrad efter likhetstecknet. Programmet känner automatiskt av vilka ord i namnet som är variabler genom att de används som variabler i funktionen (`Öka x` i exemplet). Dock kräver det att funktionen som anropas redan är definierad, så en funktion kan inte anropa funktioner som kommer senare i koden. Rekursion fungerar inte heller, men jag tror inte ens att det är tillåtet i boken. Både nyrad och semikolon kan användas för att avsluta ett funktionsanrop.

En funktion med namnet `Huvud i r` måste definieras, och det är denna funktion som körs när man kör interpreteraren. Programmet bör även returnera något i r.

Fördefinierade funktioner
-------------------------
Det finns tre fördefinierade funktioner:
* `Öka x`
* `Minska x`
* `x är icketom? i s`

`x är icketom? i s` är inte en grundläggande funktion i boken, men det blev lättare att implementera loopar om den var fördefinierad.

Definitioner av de andra funktionerna som finns med i boken finns i slutet av det här dokumentet.

Booleska uttryck
----------------
I loopar och om-satser kan man använda booleska uttryck, som används för att kontrollera om koden ska köras. De kan även användas som funktionsanrop, på formatet `(Bool)? i s`. Dock är den här syntaxen speciell, för funktionernas namn slutar med `? i s` men de används utan det slutet i de här sammanhangen. När man skriver t.ex. `Sålänge x < 12 { P }` så tar interpreteraren `x < 12` och lägger automatiskt till `? i s`, så man kan använda vilken funktion som helst som slutar med `? i s` i ett booleskt uttryck. Man kan även använda aliasen `Bool och Bool`, `Bool eller Bool` och `inte (Bool)`. Notera dock att om man vill använda ett och- eller eller-uttryck som en av boolarna i ett och- eller eller-uttryck så måste man skriva det uttrycket inom parenteser. T.ex. måste man skriva `(Bool och Bool) eller inte (Bool)` med parenteser runt `Bool och Bool`.

Notera att för nuvarande så har jag gjort det så att interpreteraren antar att alla satser som börjar med en parentes är ett funktionsanrop av formen `(Bool)? i s`, t.ex. `(x < 20 och x ≥ 10)? i r`. Detta gör uttrycket `Om x ≥ y så (Subtrahera y från x i r) annars (Subtrahera x från y i r)` från boken problematiskt, då interpreteraren skulle föredra `Om x ≥ y så Subtrahera y från x i r annars Subtrahera x från y i r`. (Boken är lite inkonsekvent när det gäller om om-satser ska ha parenteser eller ej.) Någon gång fixar jag kanske det här problemet.

Av en liknande anledning är det problematiskt att döpa en funktion till `(x < y)? i s` istället för `x < y? i s` (vilket boken av någon anledning gör fastän den inte använder parenteser i definitionen av `x ≥ y? i s`).

Funktionsdefinitioner baserade på boken
---------------------------------------
Nedan följer definitioner baserade på de som dyker upp i boken. Vissa funktioner har justerats, då boken säger att den antar att vissa justeringar görs till de här funktionerna (t.ex. `Multiplicera x och y i r`).
```
Töm x =
	Sålänge x är icketom { Minska x }
	
Flytta x till y =
	Sålänge x är icketom { Minska x; Öka y }
	
Flytta x till y och z =
	Sålänge x är icketom { Minska x; Öka y; Öka z }

Addera x till y =
	Töm e
	Flytta x till y och e
	Flytta e till x

x <- y =
	Töm x
	Addera y till x

x ← y =
	Töm x
	Addera y till x

Addera x och y i r =
	Töm r
	Addera y till r
	Addera x till r

Multiplicera x och y i r =
	x' <- x
	y' <- y
	Töm r
	Sålänge x' är icketom {
		Minska x'; Addera y' till r }

Multiplicera x med y =
	Multiplicera x och y i x

Potensupphöj x till y i r =
	y' <- y; r <- 1
	Sålänge y' är icketom {
		Minska y'
		Multiplicera r med x }

x är tom? i s =
	x' <- x; s <- 1
	Sålänge x' är icketom {
		Töm x'; Minska s }

x >= y? i s =
	x' <- x; y' <- y
	Sålänge x' är icketom {
		Minska x'; Minska y' }
	y' är tom? i s

x ≥ y? i s =
	x' <- x; y' <- y
	Sålänge x' är icketom {
		Minska x'; Minska y' }
	y' är tom? i s

Subtrahera x från y =
	x' <- x
	Sålänge x' är icketom {
		Minska y; Minska x' }

Dividera x med y i q och r =
	r <- x; q <- 0
	r >= y? i s
	Sålänge s är icketom {
		Subtrahera y från r
		Öka q
		r >= y? i s }

x < y? i s =
	inte (x >= y)? i s

x är delbar med y? i s =
	Dividera x med y i q och r
	r är tom? i s

x har någon äkta delare? i s =
	s <- 0; d <- 2
	Sålänge d < x och s är tom {
		x är delbar med d? i s
		Öka d
	}

x är prima? i s =
	(x >= 2 och inte (x har någon äkta delare))? i s
```