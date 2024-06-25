Imports System.ComponentModel.DataAnnotations
Imports System.Data
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Net.Http.Json
Imports System.Text
Imports Newtonsoft.Json.Linq


Public Enum StatutD�marche
    <Display(Name:="Brouillon")>
    brouillon
    <Display(Name:="Publi�e")>
    publiee
    <Display(Name:="Close")>
    close
    <Display(Name:="D�publi�e")>
    depubliee
End Enum

Public Enum Ordre
    <Display(Name:="Les plus r�cents")>
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

Public Class Transf�rerDossierException
    Inherits DossierException
    Public Overrides ReadOnly Property Type As String = "Transf�rerDossier"
    Public Sub New(Message As String)
        MyBase.New(Message)
    End Sub
End Class


Public Module Core

    Dim client As HttpClient = New HttpClient With {
        .BaseAddress = New Uri("https://www.demarches-simplifiees.fr/api/v2/graphql")
    }

    Friend Function Requ�terApiDS(Jeton As String, Requ�te As Object) As JObject

        'Console.WriteLine(Requ�te)

        ' Authentification
        client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", Jeton)

        ' Envoi de la requ�te
        Dim responseRaw = client.PostAsJsonAsync("", Requ�te).Result

        Dim r = responseRaw.Content.ReadAsStringAsync.Result
        

        Dim response As JObject
        ' R�cup�ration de la r�ponse (synchronis�e)
        response = JObject.Parse(r)


        ' Si elle est pr�sente, la propri�t� "data" devient la racine de l'objet r�ponse car elle renferme le contenu le plus int�ressant
        If response.ContainsKey("data") AndAlso response("data").ToObject(Of Object) IsNot Nothing Then
            response = response("data").ToObject(Of JObject)
        End If

        'Console.WriteLine(response)

        ' Mise en forme des erreurs si elles sont pr�sentes
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

        ' S'il y a des erreurs, une exception est lev�e
        If errors.Count > 0 Then
            Dim e = New Exception(String.Join(vbCrLf, errors))
            e.Data("response") = response
            Throw e
        End If

        ' Renvoi de la r�ponse
        Return response
    End Function

    'Private Function GetDossierId(Jeton As String, Num�roDossier As Integer) As String

    '    Dim r�ponse = Requ�ter(Jeton, New With {
    '        .query = My.Resources.getDossierIdByNumber,
    '        .variables = New With {
    '            .dossierNumber = Num�roDossier
    '        }
    '      })

    '    Return r�ponse("dossier")("id")

    'End Function

    Function R�cup�rerD�marche(Jeton As String, D�marche As Integer) As (String, DateTime, StatutD�marche, JArray, JArray)
    
        Dim requ�te = New With {
            .query = My.Resources.getDemarche,
            .variables = New With {
                .demarcheNumber = D�marche
            }
        }
        Dim response = Requ�terApiDS(Jeton, requ�te)
        
        Return (
            response("demarche")("activeRevision")("id").ToString,
            response("demarche")("activeRevision")("dateCreation"),
            [Enum].Parse(GetType(StatutD�marche), response("demarche")("state").ToString),
            response("demarche")("activeRevision")("champDescriptors"),
            response("demarche")("activeRevision")("annotationDescriptors")
          )

    End Function

    Function R�cup�rerDossiers(Jeton As String, D�marche As Integer, Optional Limite As Integer = 1, Optional Ordre As Ordre = Ordre.ASC, Optional Statut As StatutDossier = StatutDossier.en_construction, Optional After As DateTime = Nothing) As IEnumerable(Of Dossier)
        Dim nextPage = True
        Dim cursor As String = Nothing

        Dim dossiers = New List(Of Dossier)
        
        If After.Equals(Nothing):
            After = DateTime.MinValue
        End If

        While nextPage
            Dim requ�te = New With {
                .query = My.Resources.getDossiers,
                .variables = If(cursor Is Nothing,
                New With {
                    .demarcheNumber = D�marche,
                    .order = Ordre.GetName(Ordre),
                    .state = StatutDossier.GetName(statut),
                    .updatedSince = After.ToString("O")
                },
                New With {
                    .demarcheNumber = D�marche,
                    .order = Ordre.GetName(Ordre),
                    .state = StatutDossier.GetName(Statut),
                    .updatedSince = After.ToString("O"),
                    .after = cursor
                })
            }

            Dim response As JObject = Nothing
            Try
                response = Requ�terApiDS(Jeton, requ�te)
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
