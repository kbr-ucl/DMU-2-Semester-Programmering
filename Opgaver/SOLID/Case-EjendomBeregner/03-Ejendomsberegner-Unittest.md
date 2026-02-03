# Ejendomsberegner - Unittest



## Opgaven

Opgaven består i at lave Unittest til "Ejendomsberegner - Interface og IoC" med xUnit

Minimums krav: 

- Unittest af `BeregnKvadratmeter` metoden

- Unittest af indlæsning fra tekstfil

  
  



## Hints

Se [Hvad er adapter-mønstret?](../../../Noter/Hvad-er-adapter-moenstret.md)

- Anvend Moq
- Lav en adapter indkaspling af 
	- ```c#
    string indhold = File.ReadAllText("sti/til/fil.txt");
	  ```



