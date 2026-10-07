// See https://aka.ms/new-console-template for more information
using System;
using static System.Net.Mime.MediaTypeNames;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string L = "e^(-st)";
string iL = "-(1/s)e^-st";

Console.WriteLine("escribe la funcion");

// ejemplos de entrada para poder introducir.
Console.WriteLine("Ejemplos que puedes usar en este tipo de metodo: 5, t");
Console.WriteLine("Para sumas usa espacios para poder identificarlos: 3 + t^2");

string funcion = Console.ReadLine();

// revisamos que haya una función si no se cancela.
if (string.IsNullOrWhiteSpace(funcion))
{
    Console.WriteLine("escribe la función.");
    Console.ReadLine();
    return;
}

string[] partes = funcion.Split(' ');

string sMultiplicacion = "";

if (partes.Length == 1)
{
    string parte = partes[0];
    sMultiplicacion = $"{L}({parte})";
    Console.WriteLine($"∫{sMultiplicacion}");

    int constante = 0;

    // Evaluamos si la parte es una constante.
    if (int.TryParse(parte, out constante))
    {
        if (constante == 1)
        {
            Console.WriteLine($"∫[0,∞]{L}");

            // Integramos.
            Console.WriteLine("Integramos");
            Console.WriteLine($"∫[0,∞]{L} = {iL}");
        }
        else
        {
            Console.WriteLine($"∫[0,∞]{parte}{L}");
            Console.WriteLine("Sacamos la constante de la integral para poder integra con mas facilidad");
            Console.WriteLine($"{parte}∫[0,∞]{L}");
            Console.WriteLine("Integramos nuevamente");

            iL = iL.Replace("1", constante.ToString());

            Console.WriteLine($"{parte}∫[0,∞]{L} = {iL}");
        }

        // aclaramos que falta evaluar la antiderivada para ingresar los limites.
        Console.WriteLine(
            "La expresión obtenida es la antiderivada u tambien llamad integral; falta evaluar los límites."
        );

        // Transcribimos a integral impropia.
        Console.WriteLine("escribimos a integral impropia");
        Console.WriteLine($"lim b->∞ [{iL}][0,b]");

        // Evaluamos límites.
        Console.WriteLine("Evaluamos limites");

        // AGREGADO: sustituimos los límites de integración.
        Console.WriteLine("Sustituimos t = b y restamos el valor en t = 0:");

        Console.WriteLine(
            $"lim b->∞ [-({constante}/s)e^(-sb) + ({constante}/s)e^0]"
        );

        Console.WriteLine("Como e^0 = 1:");

        Console.WriteLine(
            $"lim b->∞ [-({constante}/s)e^(-sb) + ({constante}/s)]"
        );

        if (constante == 0)
        {
            Console.WriteLine("La función es cero.");
            Console.WriteLine("Resultado: F(s) = 0, para todo s.");
        }
        else
        {
            Console.WriteLine("Para s > 0, cuando b->∞, e^(-sb)->0.");

            Console.WriteLine(
                $"F(s) = -({constante}/s)(0) + ({constante}/s)"
            );

            Console.WriteLine($"Resultado: F(s) = {constante}/s");
            Console.WriteLine("Condición: s > 0.");
        }
    }
    // resolvemos potencias y exponenciales.
    else
    {
        string resultado = ResolverTermino(parte);

        if (resultado != "")
        {
            Console.WriteLine($"\nResultado: F(s) = {resultado}");
        }
    }
}
else
{
    for (int i = 0; i < partes.Length; i++)
    {
        string parte = partes[i];

        // AGREGADO: ignoramos espacios repetidos.
        if (parte == "")
        {
            continue;
        }

        if (parte != "+")
        {
            sMultiplicacion += $"{L}({parte})";
        }
        else
        {
            sMultiplicacion += $" {parte} ";
        }
    }

    Console.WriteLine($"∫[0,∞]{sMultiplicacion} dt");

    // AGREGADO: resolvemos cada término de la suma.
    Console.WriteLine("\nAplicamos la propiedad de linealidad:");
    Console.WriteLine(
        "La transformada de una suma es la suma de transformadas."
    );

    string resultadoFinal = "";
    bool todosValidos = true;

    for (int i = 0; i < partes.Length; i++)
    {
        string parte = partes[i];

        // Los signos + y los espacios vacíos no son términos.
        if (parte == "+" || parte == "")
        {
            continue;
        }

        string resultado = ResolverTermino(parte);

        if (resultado == "")
        {
            todosValidos = false;
            break;
        }

        if (resultadoFinal != "")
        {
            resultadoFinal += " + ";
        }

        resultadoFinal += $"({resultado})";
    }

    if (todosValidos && resultadoFinal != "")
    {
        Console.WriteLine("\nResultado final:");
        Console.WriteLine($"F(s) = {resultadoFinal}");

        Console.WriteLine(
            "El resultado es válido donde se cumplan las condiciones "
            + "de todos los términos."
        );
    }
}

// AGREGADO: evita que la consola se cierre inmediatamente.
Console.WriteLine("\nPresiona Enter para salir.");
Console.ReadLine();

// AGREGADO: función auxiliar para resolver cada término.
static string ResolverTermino(string parte)
{
    Console.WriteLine($"\n--- Resolviendo: {parte} ---");

    // Revisamos si es una constante.
    int constante;

    if (int.TryParse(parte, out constante))
    {
        if (constante == 0)
        {
            Console.WriteLine("La transformada de cero es cero.");
            return "0";
        }

        Console.WriteLine($"∫[0,∞] ({constante})e^(-st) dt");

        Console.WriteLine("Sacamos la constante:");
        Console.WriteLine($"{constante} ∫[0,∞] e^(-st) dt");

        Console.WriteLine("Escribimos la integral impropia e integramos:");

        Console.WriteLine(
            $"{constante} lim b->∞ [-e^(-st)/s][0,b]"
        );

        Console.WriteLine("Evaluamos en b y en 0:");

        Console.WriteLine(
            $"{constante} lim b->∞ [-e^(-sb)/s + 1/s]"
        );

        Console.WriteLine("Para s > 0, e^(-sb)->0.");
        Console.WriteLine($"Resultado del término: {constante}/s");

        return $"{constante}/s";
    }

    // Identificamos t, t^2, 3t^2, -2t, etc.
    var potencia = System.Text.RegularExpressions.Regex.Match(
        parte,
        @"^(?<c>[+-]?(?:\d+\*?)?)t(?:\^(?<n>\d+))?$"
    );

    if (potencia.Success)
    {
        string textoCoeficiente =
            potencia.Groups["c"].Value.Replace("*", "");

        if (textoCoeficiente == "" || textoCoeficiente == "+")
        {
            textoCoeficiente = "1";
        }

        if (textoCoeficiente == "-")
        {
            textoCoeficiente = "-1";
        }

        int coeficiente;
        int n = 1;

        if (!int.TryParse(textoCoeficiente, out coeficiente))
        {
            Console.WriteLine("El coeficiente es demasiado grande.");
            return "";
        }

        if (potencia.Groups["n"].Success)
        {
            if (!int.TryParse(potencia.Groups["n"].Value, out n)
                || n > 20)
            {
                Console.WriteLine(
                    "Usa una potencia entera entre 0 y 20."
                );

                return "";
            }
        }

        if (coeficiente == 0)
        {
            Console.WriteLine(
                "El término es cero. Su transformada es 0."
            );

            return "0";
        }

        Console.WriteLine($"∫[0,∞] e^(-st)({parte}) dt");

        Console.WriteLine("Sacamos el coeficiente:");

        Console.WriteLine(
            $"{coeficiente} lim b->∞ ∫[0,b] t^{n}e^(-st) dt"
        );

        Console.WriteLine("Condición: s > 0.");

        if (n > 0)
        {
            Console.WriteLine("\nUsamos integración por partes:");
            Console.WriteLine("∫u dv = uv - ∫v du");

            Console.WriteLine(
                "Definimos I_k(b) = ∫[0,b] t^k e^(-st) dt."
            );

            for (int k = n; k >= 1; k--)
            {
                Console.WriteLine($"\nPara la potencia {k}:");

                Console.WriteLine($"u = t^{k}");
                Console.WriteLine($"du = {k}t^{k - 1} dt");
                Console.WriteLine("dv = e^(-st) dt");
                Console.WriteLine("v = -e^(-st)/s");

                Console.WriteLine(
                    $"I_{k}(b) = [-t^{k}e^(-st)/s][0,b]"
                    + $" + ({k}/s)I_{k - 1}(b)"
                );

                Console.WriteLine(
                    $"El término evaluado es -b^{k}e^(-sb)/s."
                );

                Console.WriteLine(
                    "Cuando b->∞, ese término tiende a 0 porque s > 0."
                );
            }
        }

        Console.WriteLine("\nResolvemos la integral base:");

        Console.WriteLine("I_0(b) = [-e^(-st)/s][0,b]");
        Console.WriteLine("I_0(b) = (1 - e^(-sb))/s");
        Console.WriteLine("Cuando b->∞, I_0(b)->1/s.");

        // Calculamos el factorial y mostramos las sustituciones.
        decimal factorial = 1;

        for (int k = 1; k <= n; k++)
        {
            factorial *= k;

            Console.WriteLine(
                $"Sustituyendo: lim b->∞ I_{k}(b)"
                + $" = {factorial}/s^{k + 1}"
            );
        }

        decimal numerador = coeficiente * factorial;
        string denominador = n == 0 ? "s" : $"s^{n + 1}";

        Console.WriteLine("\nMultiplicamos por el coeficiente:");

        Console.WriteLine(
            $"{coeficiente} * {factorial}/{denominador}"
            + $" = {numerador}/{denominador}"
        );

        return $"{numerador}/{denominador}";
    }

    // Identificamos e^t, e^(2t), e^(-t), 3e^(2t), etc.
    var exponencial = System.Text.RegularExpressions.Regex.Match(
        parte,
        @"^(?<c>[+-]?(?:\d+\*?)?)e\^(?:t|\((?<a>[+-]?(?:\d+\*?)?)t\))$"
    );

    if (exponencial.Success)
    {
        string textoCoeficiente =
            exponencial.Groups["c"].Value.Replace("*", "");

        string textoExponente =
            exponencial.Groups["a"].Value.Replace("*", "");

        if (textoCoeficiente == "" || textoCoeficiente == "+")
        {
            textoCoeficiente = "1";
        }

        if (textoCoeficiente == "-")
        {
            textoCoeficiente = "-1";
        }

        if (textoExponente == "" || textoExponente == "+")
        {
            textoExponente = "1";
        }

        if (textoExponente == "-")
        {
            textoExponente = "-1";
        }

        int coeficiente;
        int a;

        if (!int.TryParse(textoCoeficiente, out coeficiente)
            || !int.TryParse(textoExponente, out a))
        {
            Console.WriteLine(
                "El coeficiente o el exponente es demasiado grande."
            );

            return "";
        }

        if (coeficiente == 0)
        {
            Console.WriteLine(
                "El término es cero. Su transformada es 0."
            );

            return "0";
        }

        string q = "s";

        if (a > 0)
        {
            q = $"(s - {a})";
        }

        if (a < 0)
        {
            q = $"(s + {-(long)a})";
        }

        Console.WriteLine($"∫[0,∞] e^(-st)({parte}) dt");
        Console.WriteLine($"Sacamos el coeficiente {coeficiente}.");

        Console.WriteLine("Multiplicamos las exponenciales:");

        Console.WriteLine(
            $"e^(-st) * e^({a}t) = e^((-s + ({a}))t)"
        );

        Console.WriteLine($"Esto equivale a e^(-{q}t).");

        Console.WriteLine($"\nDefinimos q = {q}.");

        Console.WriteLine(
            $"{coeficiente} lim b->∞ ∫[0,b] e^(-qt) dt"
        );

        Console.WriteLine("\nIntegramos por sustitución:");

        Console.WriteLine("u = -qt");
        Console.WriteLine("du = -q dt");
        Console.WriteLine("dt = -du/q");
        Console.WriteLine("∫e^(-qt) dt = -(1/q)∫e^u du");
        Console.WriteLine("= -e^u/q = -e^(-qt)/q");

        Console.WriteLine("\nEvaluamos los límites de infinito y 0:");

        Console.WriteLine(
            $"{coeficiente} lim b->∞ [-e^(-qt)/q][0,b]"
        );

        Console.WriteLine(
            $"{coeficiente} lim b->∞ [-e^(-qb)/q + 1/q]"
        );

        Console.WriteLine("Si q > 0, entonces e^(-qb)->0.");

        Console.WriteLine($"Resultado: {coeficiente}/{q}");
        Console.WriteLine($"Condición: s > {a}.");

        return $"{coeficiente}/{q}";
    }

    Console.WriteLine("No se reconoce el término: " + parte);

    Console.WriteLine(
        "Ejemplos válidos: 5, t, 3t^2, e^t, 2e^(-t)."
    );

    return "";
}