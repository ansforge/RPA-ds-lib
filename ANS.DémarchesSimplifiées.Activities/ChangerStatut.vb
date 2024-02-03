Imports System.Activities
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq

<DisplayName("Changer le statut d’un dossier")>
Public Class ChangerStatut
    Inherits CodeActivity

    <Category("Input")>
    <DisplayName("Jeton d’authentification")>
    <RequiredArgument()>
    Public Property Jeton As InArgument(Of String)
    
    <Category("Input")>
    <DisplayName("Instructeur")>
    <RequiredArgument()>
    Public Property Instructeur As InArgument(Of String)

    <Category("Input")>
    <DisplayName("Numéro de dossier")>
    <RequiredArgument()>
    Public Property NuméroDossier As InArgument(Of Integer)

    <Category("Input")>
    <DisplayName("Statut")>
    <RequiredArgument()>
    Public Property Statut As InArgument(Of StatutDossier)
    
    <Category("Input")>
    <DisplayName("Motivation")>
    Public Property Motivation As InArgument(Of String)
    
    <Category("Input")>
    <DisplayName("Notifier le demandeur")>
    Public Property Notifier As Boolean = True
    
    <Category("Output")>
    <DisplayName("Résultat")>
    Public Property Résultat As OutArgument(Of JObject)

    Protected Overrides Sub Execute(context As CodeActivityContext)
        Résultat.Set(context,
            Core.ChangerStatut(
                Jeton.Get(context),
                Instructeur.Get(context),
                NuméroDossier.Get(context),
                Statut.Get(context),
                Motivation.Get(context),
                Notifier
            )
        )
    End Sub

End Class