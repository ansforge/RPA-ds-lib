Imports System.Activities.Presentation.Metadata
Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Controls

Imports System.ComponentModel.DataAnnotations
Imports System.Reflection
Imports System.Windows.Data
Imports System.Globalization

Public Class ActivityDecoratorControl
    Inherits ContentControl

    Public Shared Sub ActivityDecoratorControl()
        DefaultStyleKeyProperty.OverrideMetadata(GetType(ActivityDecoratorControl), New FrameworkPropertyMetadata(GetType(ActivityDecoratorControl)))
    End Sub

End Class

Public Class EnumValueConverter
    Implements IValueConverter

    Private Shared Function GetDisplayName(EnumValue As [Enum]) As String
        Dim displayAttribute = EnumValue.GetType.GetMember([Enum].GetName(EnumValue.GetType, EnumValue)).First.GetCustomAttribute(Of DisplayAttribute)
        If displayAttribute IsNot Nothing Then
            Return displayAttribute.Name
        End If
        Return EnumValue.ToString
    End Function

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim enumValue As [Enum] = value
        If enumValue IsNot Nothing Then
            Return GetDisplayName(enumValue)
        End If
        Return value.ToString
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException() 'Return value
    End Function
End Class

Public Class DesignerMetadata
    Implements IRegisterMetadata

    Dim Catégorie = "ANS.Démarches_Simplifiées"
    Public Sub Register() Implements IRegisterMetadata.Register
        Dim builder = New AttributeTableBuilder()
        builder.AddCustomAttributes(GetType(ChangerStatut), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(ChangerStatut), New DesignerAttribute(GetType(ChangerStatutDesigner)))

        builder.AddCustomAttributes(GetType(TransférerDossier), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(TransférerDossier), New DesignerAttribute(GetType(TransférerDossierDesigner)))

        'builder.AddCustomAttributes(GetType(ChargerPiècesJointes), New CategoryAttribute(Catégorie))
        'builder.AddCustomAttributes(GetType(ChargerPiècesJointes), New DesignerAttribute(GetType(ChargerDossierDesigner)))

        builder.AddCustomAttributes(GetType(RécupérerDossiers), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(RécupérerDossiers), New DesignerAttribute(GetType(RécupérerDossiersDesigner)))

        builder.AddCustomAttributes(GetType(RécupérerDémarche), New CategoryAttribute(Catégorie))

        builder.AddCustomAttributes(GetType(AnnoterDossier), New CategoryAttribute(Catégorie))
        builder.AddCustomAttributes(GetType(AnnoterDossier), New DesignerAttribute(GetType(AnnoterDossierDesigner)))
        
        builder.AddCustomAttributes(GetType(SFLogMessage), New CategoryAttribute(Catégorie))

        MetadataStore.AddAttributeTable(builder.CreateTable())

    End Sub
End Class
