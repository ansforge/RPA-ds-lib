Imports System.ComponentModel.DataAnnotations
Imports System.Data
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Net.Http.Json
Imports System.Text
Imports Newtonsoft.Json.Linq


Public Enum StatutDémarche
    <Display(Name:="Brouillon")>
    brouillon
    <Display(Name:="Publiée")>
    publiee
    <Display(Name:="Close")>
    close
    <Display(Name:="Dépubliée")>
    depubliee
End Enum

Public Enum Ordre
    <Display(Name:="Les plus récents")>
    DESC
    <Display(Name:="Les plus anciens")>
    ASC
End Enum

Public MustInherit Class DossierException
    Inherits Exception

    Public MustOverride ReadOnly Property Type As String
    Public Sub New(Message As String)
        MyBase.New(Message)
    End Sub
End Class

Public Class RetournerDossierException
    Inherits DossierException
    Public Overrides ReadOnly Property Type As String = "RetournerDossier"
    Public Sub New(Message As String)
        MyBase.New(Message)
    End Sub
End Class

Public Class RefuserDossierException
    Inherits DossierException
    Public Overrides ReadOnly Property Type As String = "RefuserDossier"
    Public Sub New(Message As String)
        MyBase.New(Message)
    End Sub
End Class

Public Class TransférerDossierException
    Inherits DossierException
    Public Overrides ReadOnly Property Type As String = "TransférerDossier"
    Public Sub New(Message As String)
        MyBase.New(Message)
    End Sub
End Class


Public Module Core

    Dim client As HttpClient = New HttpClient With {
        .BaseAddress = New Uri("https://www.demarches-simplifiees.fr/api/v2/graphql")
    }

    Friend Function RequêterApiDS(Jeton As String, Requête As Object) As JObject

        'Console.WriteLine(Requête)

        ' Authentification
        client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", Jeton)

        ' Envoi de la requête
        Dim responseRaw = client.PostAsJsonAsync("", Requête).Result

        Dim r = responseRaw.Content.ReadAsStringAsync.Result
        

        Dim response As JObject
        ' Récupération de la réponse (synchronisée)
        response = JObject.Parse(r)


        ' Si elle est présente, la propriété "data" devient la racine de l'objet réponse car elle renferme le contenu le plus intéressant
        If response.ContainsKey("data") AndAlso response("data").ToObject(Of Object) IsNot Nothing Then
            response = response("data").ToObject(Of JObject)
        End If

        'Console.WriteLine(response)

        ' Mise en forme des erreurs si elles sont présentes
        Dim errors = New List(Of String)
        If response.ContainsKey("errors") AndAlso response("errors").ToObject(Of Object) IsNot Nothing Then
            errors.Add(String.Join(" ; ", From e As JObject In response("errors") Select e("message")))
        Else
            For Each item In response
                Dim value = item.Value.ToObject(Of JObject)
                If value.ContainsKey("errors") AndAlso value("errors").ToObject(Of Object) IsNot Nothing Then
                    errors.Add(item.Key + " : " + String.Join(" ; ", From e As JObject In value("errors") Select """" + e("message").ToString + """"))
                End If
            Next
        End If

        ' S'il y a des erreurs, une exception est levée
        If errors.Count > 0 Then
            Dim e = New Exception(String.Join(vbCrLf, errors))
            e.Data("response") = response
            Throw e
        End If

        ' Renvoi de la réponse
        Return response
    End Function

    'Private Function GetDossierId(Jeton As String, NuméroDossier As Integer) As String

    '    Dim réponse = Requêter(Jeton, New With {
    '        .query = My.Resources.getDossierIdByNumber,
    '        .variables = New With {
    '            .dossierNumber = NuméroDossier
    '        }
    '      })

    '    Return réponse("dossier")("id")

    'End Function

    Function RécupérerDémarche(Jeton As String, Démarche As Integer) As (String, DateTime, StatutDémarche, JArray, JArray)
    
        Dim requête = New With {
            .query = My.Resources.getDemarche,
            .variables = New With {
                .demarcheNumber = Démarche
            }
        }
        Dim response = RequêterApiDS(Jeton, requête)
        
        Return (
            response("demarche")("activeRevision")("id").ToString,
            response("demarche")("activeRevision")("dateCreation"),
            [Enum].Parse(GetType(StatutDémarche), response("demarche")("state").ToString),
            response("demarche")("activeRevision")("champDescriptors"),
            response("demarche")("activeRevision")("annotationDescriptors")
          )

    End Function

    Function RécupérerDossiers(Jeton As String, Démarche As Integer, Optional Limite As Integer = 1, Optional Ordre As Ordre = Ordre.ASC, Optional Statut As StatutDossier = StatutDossier.en_construction, Optional After As DateTime = Nothing) As IEnumerable(Of Dossier)
        Dim nextPage = True
        Dim cursor As String = Nothing

        Dim dossiers = New List(Of Dossier)
        
        If After.Equals(Nothing):
            After = DateTime.MinValue
        End If

        While nextPage
            Dim requête = New With {
                .query = My.Resources.getDossiers,
                .variables = If(cursor Is Nothing,
                New With {
                    .demarcheNumber = Démarche,
                    .order = Ordre.GetName(Ordre),
                    .state = StatutDossier.GetName(statut),
                    .updatedSince = After.ToString("O")
                },
                New With {
                    .demarcheNumber = Démarche,
                    .order = Ordre.GetName(Ordre),
                    .state = StatutDossier.GetName(Statut),
                    .updatedSince = After.ToString("O"),
                    .after = cursor
                })
            }

            Dim response As JObject = Nothing
            Try
                response = RequêterApiDS(Jeton, requête)
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Throw
            End Try
            
            cursor = response("demarche")("dossiers")("pageInfo")("endCursor")
            nextPage = response("demarche")("dossiers")("pageInfo")("hasNextPage")

            
            For Each jDossier As Object In response("demarche")("dossiers")("nodes")
                If jDossier IsNot Nothing Then
                    'Console.WriteLine(jDossier.ToString)
                    dossiers.Add(New Dossier(jDossier))
                    If dossiers.Count.Equals(Limite) Then
                        Exit While
                    End If
                End If
            Next
            
        End While
        
        Return dossiers

    End Function

End Module
