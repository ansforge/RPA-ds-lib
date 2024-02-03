Imports System.Activities
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq

<DisplayName("Lister les champs d'une démarche")>
Public Class RécupérerDémarche
    Inherits CodeActivity

    <Category("Input")>
    <DisplayName("Jeton d’authentification")>
    <RequiredArgument()>
    Public Property Jeton As InArgument(Of String)

    <Category("Input")>
    <DisplayName("Numéro de démarche")>
    <RequiredArgument()>
    Public Property Démarche As InArgument(Of Integer)

    <Category("Output")>
    <DisplayName("Liste des champs")>
    Public Property Champs As OutArgument(Of JArray)
    
    <Category("Output")>
    <DisplayName("Liste des annotations")>
    Public Property Annotations As OutArgument(Of JArray)

    <Category("Output")>
    <DisplayName("Date de la révision")>
    Public Property DateRévision As OutArgument(Of DateTime)

    <Category("Output")>
    <DisplayName("État de la révision")>
    Public Property ÉtatRévision As OutArgument(Of StatutDémarche)

    Protected Overrides Sub Execute(context As CodeActivityContext)
        Dim t = Core.RécupérerDémarche(Jeton.Get(context), Démarche.Get(context))
        Champs.Set(context, t.Item1)
        Annotations.Set(context, t.Item2)
        DateRévision.Set(context, t.Item3)
        ÉtatRévision.Set(context, t.Item4)
    End Sub

End Class