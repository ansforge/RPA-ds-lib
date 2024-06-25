Imports System.Activities
Imports System.ComponentModel

<DisplayName("Récupérer plusieurs dossiers")>
Public Class RécupérerDossiers
    Inherits CodeActivity

    <Category("Input")>
    <DisplayName("Jeton d’authentification")>
    <RequiredArgument()>
    Public Property Jeton As InArgument(Of String)

    <Category("Input")>
    <DisplayName("Numéro de démarche")>
    <RequiredArgument()>
    Public Property NuméroDémarche As InArgument(Of Integer)

    <Category("Input")>
    <DisplayName("Nombre maximum de dossiers")>
    <RequiredArgument()>
    Public Property Limite As InArgument(Of Integer) = 1

    <Category("Input")>
    <DisplayName("Ordre")>
    <RequiredArgument()>
    Public Property Ordre As Ordre = Ordre.ASC

    <Category("Input")>
    <DisplayName("Statut")>
    <RequiredArgument()>
    Public Property Statut As StatutDossier = StatutDossier.en_construction
    
    <Category("Input")>
    <DisplayName("À partir d’une date")>
    Public Property After As InArgument(Of DateTime) = Nothing

    <Category("Output")>
    <DisplayName("Dossiers récupérés")>
    <RequiredArgument()>
    Public Property Dossiers As OutArgument(Of IEnumerable(Of Dossier))

    Protected Overrides Sub Execute(context As CodeActivityContext)
        Dossiers.Set(context, Core.RécupérerDossiers(Jeton.Get(context), NuméroDémarche.Get(context), Limite.Get(context), Ordre, Statut, After.Get(context)))
    End Sub

End Class