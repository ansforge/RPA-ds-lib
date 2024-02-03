Imports System.Activities
Imports System.ComponentModel

<DisplayName("Annoter un dossier")>
Public Class AnnoterDossier
    Inherits CodeActivity

    <Category("Input")>
    <DisplayName("Jeton d’authentification")>
    <RequiredArgument()>
    Public Property Jeton As InArgument(Of String)
    
    <Category("Input")>
    <DisplayName("Numéro de dossier")>
    <RequiredArgument()>
    Public Property NuméroDossier As InArgument(Of Integer)
    
    <Category("Input")>
    <DisplayName("Instructeur")>
    <RequiredArgument()>
    Public Property Instructeur As InArgument(Of String)
    
    <Category("Input")>
    <DisplayName("Label du champ à annoter")>
    <RequiredArgument()>
    Public Property ChampAnnotation As InArgument(Of String)

    <Category("Input")>
    <DisplayName("Texte")>
    <RequiredArgument()>
    Public Property Texte As InArgument(Of String)


    Protected Overrides Sub Execute(context As CodeActivityContext)
        'If IdentifiantDossier IsNot Nothing Then
        '    Throw New Exception("La recherche de dossier par ID n'est pas encore implémentée.")
        'End If
        Core.AnnoterDossier(Jeton.Get(context), NuméroDossier.Get(context), Instructeur.Get(context), ChampAnnotation.Get(context), Texte.Get(context))
    End Sub

End Class