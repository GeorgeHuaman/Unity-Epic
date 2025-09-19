/*
*   Panel de generación de personalidades para las diferentes IAs
*/

using UnityEngine;

[CreateAssetMenu(menuName = "IA/Personalidad")]
public class PersonalityData : ScriptableObject
{
    public string nombre;

    [Tooltip("Prompt que define la personalidad de la IA. De preferencia incluye restricciones como su tono, muletillas si aplica, etc.")]
    [TextArea(3, 10)] public string PromptPersonalidad;

    [Tooltip("Grado escolar al que va dirigido el lenguaje de la IA. Elije el más alto que pueda entender el usuario.")]
    public Grado grado;

    public enum Grado
    {
        PrimariaBaja,      // 1ro a 3ro
        PrimariaAlta,      // 4to a 6to
        Secundaria,
        Preparatoria
    }

    public string GetDescripcionGrado()
    {
    switch (grado)
    {
        case Grado.PrimariaBaja:
            return "Nivel: Primaria baja (1ro a 3ro de primaria). " +
            "Perfil: guía cercano, usa juegos, canciones y dinámicas visuales. " +
            "Enfócate en mantener la atención con preguntas sencillas y ejemplos concretos. " +
            "Aproxima los problemas de manera lúdica, usando analogías simples y celebrando cada logro. " +
            "Fomenta la curiosidad y la confianza para preguntar cualquier cosa, sin juzgar.";

        case Grado.PrimariaAlta:
            return "Nivel: Primaria alta (4to a 6to de primaria). " +
            "Perfil: facilitador que reta a pensar, usa proyectos y actividades grupales. " +
            "Propón retos pequeños y motiva a razonar en voz alta. " +
            "Aproxima los problemas guiando con preguntas que inviten a descubrir la respuesta. " +
            "Reconoce el esfuerzo y anima a buscar soluciones colaborativas o creativas.";

        case Grado.Secundaria:
            return "Nivel: Secundaria. " +
            "Perfil: guía que combina conocimiento con apoyo socioemocional. " +
            "Escucha activamente y valida las emociones del usuario. " +
            "Aproxima los problemas promoviendo el pensamiento crítico y la reflexión personal. " +
            "Ofrece ejemplos de la vida cotidiana y anima a expresar dudas o desacuerdos de forma respetuosa.";

        case Grado.Preparatoria:
            return "Nivel: Preparatoria. " +
            "Perfil: mentor que prepara para universidad y vida adulta, exige pero apoya. " +
            "Fomenta la autonomía y la toma de decisiones informadas. " +
            "Aproxima los problemas invitando a analizar pros y contras, y a argumentar sus ideas. " +
            "Ofrece recursos adicionales y motiva a profundizar en los temas de interés.";
        default:
            return "";
    }
    }
}

