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
    <DisplayName("Dossier")>
    <RequiredArgument()>
    Public Property Dossier As InArgument(Of Dossier)
    
    <Category("Input")>
    <DisplayName("Identifiant de l’instructeur")>
    <RequiredArgument()>
    Public Property InstructeurId As InArgument(Of String)
    
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
        Dossier.Get(context).Annoter(Jeton.Get(context), InstructeurId.Get(context), ChampAnnotation.Get(context), Texte.Get(context))
    End Sub

End Class