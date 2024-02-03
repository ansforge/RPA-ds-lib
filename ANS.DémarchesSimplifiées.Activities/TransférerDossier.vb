Imports System.Activities
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq

<DisplayName("Transférer le dossier à un autre instructeur")>
Public Class TransférerDossier
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


    Protected Overrides Sub Execute(context As CodeActivityContext)
        Core.TransférerDossier(
            Jeton.Get(context),
            Instructeur.Get(context),
            NuméroDossier.Get(context)
        )
    End Sub

End Class