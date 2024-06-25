Imports System.Activities
Imports System.ComponentModel

<DisplayName("Changer le statut d’un dossier")>
Public Class ChangerStatut
    Inherits CodeActivity

    <Category("Input")>
    <DisplayName("Jeton d’authentification")>
    <RequiredArgument()>
    Public Property Jeton As InArgument(Of String)
    
    <Category("Input")>
    <DisplayName("Dossier")>
    <RequiredArgument()>
    Public Property Dossier As InArgument(Of Dossier)

    <Category("Input")>
    <DisplayName("Identifiant de l’instructeur")>
    <RequiredArgument()>
    Public Property InstructeurId As InArgument(Of String)

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
    
    Protected Overrides Sub Execute(context As CodeActivityContext)
        Dossier.Get(context).ChangerStatut(
            Jeton.Get(context),
            InstructeurId.Get(context),
            Statut.Get(context),
            Motivation.Get(context),
            Notifier
        )
    End Sub

End Class