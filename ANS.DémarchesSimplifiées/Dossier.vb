Imports Newtonsoft.Json.Linq
Imports System.ComponentModel.DataAnnotations
Imports System.Net.Http
Imports System.Text

Public Enum StatutDossier
    <Display(Name:="En construction")>
    en_construction
    <Display(Name:="En instruction")>
    en_instruction
    <Display(Name:="Accepté")>
    accepte
    <Display(Name:="Refusé")>
    refuse
    <Display(Name:="Sans suite")>
    sans_suite
End Enum

Public Class Dossier
    Inherits JObject
    
    Public ReadOnly Numéro As Integer
    Public ReadOnly Identifiant As String
    Private _Statut As StatutDossier
    Public Property Statut As StatutDossier
        Get
            Return _Statut
        End Get
        Private Set(value As StatutDossier)
            _Statut = Statut
        End Set
    End Property

    Private ids As IEnumerable(Of String)
    Private labels As Dictionary(Of String, String)

    Private Shared Function FromApi(Jeton As String, Numéro As Integer) As JObject
        Dim jDossier = RequêterApiDS(Jeton, New With {
            .query = My.Resources.getDossier,
            .variables = New With {
                .dossierNumber = Numéro
            }
          })("dossier")
        If jDossier Is Nothing Then
            Throw New Exception($"Impossible de récupérer le dossier {Numéro}.")
        End If
        Return jDossier
    End Function

    Public Sub New(Jeton As String, Numéro As Integer)
        MyBase.New(FromApi(Jeton, Numéro))

        For Each field As JObject In Me("champs")
            If field.ContainsKey("files") Then
                For Each file As JObject In field("files")
                    If file.ContainsKey("filename") And file.ContainsKey("url") And Not String.IsNullOrEmpty(file.Value(Of String)("url")) Then
                        Dim client = New HttpClient()
                        file("filebytes") = client.GetAsync(file.Value(Of String)("url")).Result.Content.ReadAsByteArrayAsync.Result
                    End If
                Next
            End If
        Next
    End Sub

    Public Sub New(other As JObject)
        MyBase.New(other)

        If Not ContainsKey("champs") Then
            Throw New Exception("Erreur lors de la conversion du dossier: la liste des champs n’a pas été trouvée.")
        End If
        ids = From field As JToken In Item("champs") Let s = field("id").ToString Select s
        
        Numéro = other("number")
        Identifiant = other("id")

        Statut = [Enum].Parse(GetType(StatutDossier), other("state").ToString)
    End Sub

    Public Property LabelsChamps() As Dictionary(Of String, String)
        Get
            Return labels
        End Get
        
        Set(value As Dictionary(Of String, String))
            For Each id In value.Values
                If Not ids.Contains(id) Then
                    Throw New Exception($"Erreur lors de la labellisation: le dossier n’a pas de champ {id}.")
                End If
            Next
            labels = value
        End Set

    End Property

    Function Champ(Nom As String) As JToken
        If LabelsChamps Is Nothing Then
            Throw New Exception("Les champs de ce dossier n’ont pas reçu de labels.")
        ElseIf Not LabelsChamps.ContainsKey(Nom) Then
            Throw New Exception($"Pas de champ labellisé ""{Nom}"" dans le dossier.")
        End If
        For Each field As JObject In Item("champs")
            If field("id").ToString.Equals(LabelsChamps(Nom)) Then
                Return field
            End If
        Next
        Throw New Exception($"Le champ ""{Nom}"" n’a pas été trouvé pour ce dossier.")
    End Function

    Function ChampExiste(Nom As String) As Boolean
        If LabelsChamps Is Nothing Then
            Throw New Exception("Les champs de ce dossier n’ont pas été labellisés.")
        End If
        If LabelsChamps.ContainsKey(Nom) Then
            For Each field As JObject In Item("champs")
                If field("id").ToString.Equals(LabelsChamps(Nom)) Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function


    Shared Function Normaliser(InputString As String) As String
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
        Dim tempBytes = Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(InputString.ToUpper)
        Return Text.Encoding.UTF8.GetString(tempBytes).Replace("'"," ")
    End Function

    Function ChampStr(Nom As String) As String
        Return Normaliser(Champ(Nom).Value(Of String)("stringValue"))
    End Function
    
    Function ChampInt(Nom As String) As Integer
        Return Champ(Nom).Value(Of Integer)("stringValue")
    End Function
    
    Function ChampBool(Nom As String) As Boolean
        Return Champ(Nom).Value(Of Boolean)("stringValue")
    End Function

    Function ChampParId(Id As String) As JToken
        For Each field As JObject In Item("champs")
            If field("id").ToString.Equals(Id) Then
                Return field
            End If
        Next
        Throw New Exception($"Le champ d’identifiant ""{Id}"" n’a pas été trouvé pour ce dossier.")
    End Function

    Function ChampParLabel(Label As String) As JToken
        For Each field As JObject In Item("champs")
            If field("label").ToString.Equals(Label) Then
                Return field
            End If
        Next
        Throw New Exception($"Le champ labellisé ""{Label}"" n’a pas été trouvé pour ce dossier.")
    End Function

    Sub ChangerStatut(Jeton As String, InstructeurId As String, Statut As StatutDossier, Optional Motivation As String = Nothing, Optional Notifier As Boolean = True)

        Dim dossierId = Identifiant

        ' Assemblage des paramètres...
        Dim query = Nothing
        Dim disableNotification = Not Notifier
        Dim input As Object = New With {
            instructeurId,
            dossierId,
            motivation,
            disableNotification
        }

        ' ...en fonction du type de changement
        Select Case Statut
            Case StatutDossier.en_construction
                query = My.Resources.dossierRepasserEnConstruction
                input = New With {instructeurId, dossierId}
            Case StatutDossier.en_instruction
                If {"accepte","refuse","sans_suite"}.Contains(Me.Statut) Then
                    query = My.Resources.dossierRepasserEnInstruction
                Else
                    query = My.Resources.dossierPasserEnInstruction
                End If
                input = New With {instructeurId, dossierId}
            Case StatutDossier.accepte
                query = My.Resources.dossierAccepter
            Case StatutDossier.refuse
                query = My.Resources.dossierRefuser
            Case StatutDossier.sans_suite
                query = My.Resources.dossierClasserSansSuite
        End Select
        Dim variables = New With {input}


        ' Requête vers l'API
        Dim response = RequêterApiDS(Jeton, New With {query, variables})

        Me._Statut = Statut
        'Console.WriteLine("1) "+Statut.ToString+" => "+Me.Statut.ToString)
    End Sub


    Public Sub Transférer(Jeton As String, InstructeurId As String)

        ' Assemblage des paramètres...
        Dim input As Object = New With {
            .dossierId = Identifiant,
            instructeurId,
            .disableNotification = True
        }
        Dim variables = New With {input}

        ' Requête vers l'API
        RequêterApiDS(Jeton, New With {.query = My.Resources.dossierRepasserEnConstruction, variables})
        RequêterApiDS(Jeton, New With {.query = My.Resources.dossierPasserEnInstruction, variables})

    End Sub

    Public Sub Annoter(Jeton As String, InstructeurId As String, ChampAnnotation As String, Texte As String)

        Dim annotationId As String = Nothing
        For Each annotation As JObject In Me("annotations")
            If annotation("label").ToString.Equals(ChampAnnotation) Then
                annotationId = annotation("id")
                Exit For
            End If
        Next
        
        If annotationId Is Nothing Then
            Throw New Exception($"L'annotation de label ""{ChampAnnotation}"" n'a pas été trouvée pour le dossier numéro {Numéro}.")
        End If


        ' Assemblage des paramètres...
        Dim input As Object = New With {
            .dossierId = Identifiant,
            instructeurId,
            annotationId,
            .value = Texte
        }
        Dim variables = New With {input}

        ' Requête vers l'API
        RequêterApiDS(Jeton, New With {.query = My.Resources.dossierModifierAnnotationText, variables})

    End Sub
End Class