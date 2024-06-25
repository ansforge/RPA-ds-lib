Imports System.Activities
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq

<DisplayName("Récupérer la dernière révision d’une démarche")>
Public Class RécupérerDémarche
    Inherits CodeActivity

    <Category("Input")>
    <DisplayName("Jeton d’authentification")>
    <RequiredArgument()>
    Public Property Jeton As InArgument(Of String)

    <Category("Input")>
    <DisplayName("Numéro de démarche")>
    <RequiredArgument()>
    Public Property NuméroDémarche As InArgument(Of Integer)
   
    <Category("Output")>
    <DisplayName("Identifiant de la révision")>
    Public Property IdRévision As OutArgument(Of String)

    <Category("Output")>
    <DisplayName("Date de la révision")>
    Public Property DateRévision As OutArgument(Of DateTime)

    <Category("Output")>
    <DisplayName("État de la révision")>
    Public Property ÉtatRévision As OutArgument(Of StatutDémarche)

    <Category("Output")>
    <DisplayName("Liste des champs")>
    Public Property Champs As OutArgument(Of JArray)
    
    <Category("Output")>
    <DisplayName("Liste des annotations")>
    Public Property Annotations As OutArgument(Of JArray)
 
    Protected Overrides Sub Execute(context As CodeActivityContext)
        Dim t = Core.RécupérerDémarche(Jeton.Get(context), NuméroDémarche.Get(context))
        IdRévision.Set(context, t.Item1)
        DateRévision.Set(context, t.Item2)
        ÉtatRévision.Set(context, t.Item3)
        Champs.Set(context, t.Item4)
        Annotations.Set(context, t.Item5)
    End Sub

End Class