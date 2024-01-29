Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Public Class ComposerAliases : Inherits BindingList(Of ComposerAlias) : Implements ISupportHasChanges, INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            For Each comp In Me
                If comp.HasChanges Then Return True
            Next
            Return False
        End Get
        Set(value As Boolean)
            For Each comp In Me
                comp.HasChanges = value
            Next
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property

    Friend Sub SyncWith(foundRecommendation As Recommendation)
        If foundRecommendation Is Nothing OrElse foundRecommendation.Composer = Nothing Then Exit Sub

        Dim foundAlias = Me.FindComposer(foundRecommendation)
        If foundAlias Is Nothing Then
            ' Doesn't exist yet
            Dim newAlias As New ComposerAlias(foundRecommendation.Composer, foundRecommendation.AlternateComposerSpellings)
            Me.Add(newAlias)
        Else
            ' Does exist
            Dim allComposerNames As New List(Of String)
            allComposerNames.Add(foundAlias.PrimaryName)
            allComposerNames.Add(foundRecommendation.Composer)
            allComposerNames.AddRange(foundAlias.Aliases)
            allComposerNames.AddRange(foundRecommendation.AlternateComposerSpellings)
            allComposerNames = allComposerNames.Distinct.ToList

            allComposerNames.Remove(foundAlias.PrimaryName)
            allComposerNames.Sort()

            foundRecommendation.Composer = foundAlias.PrimaryName

            If Not ListsAreSame(foundRecommendation.AlternateComposerSpellings, allComposerNames) Then
                foundRecommendation.AlternateComposerSpellings.Clear()
                For Each name In allComposerNames
                    foundRecommendation.AlternateComposerSpellings.Add(name)
                Next
            End If

            If Not ListsAreSame(foundAlias.Aliases, allComposerNames) Then
                foundAlias.Aliases.Clear()
                For Each name In allComposerNames
                    foundAlias.Aliases.Add(name)
                Next
            End If

        End If
    End Sub

    Friend Function FindComposer(rec As IComposer) As ComposerAlias


        Return Me.ToList.Find(Function(x)
                                  Return x.Equals(rec)
                              End Function)
    End Function
End Class

Public Class ComposerAlias : Implements INotifyPropertyChanged, IComposer, IComposerMetadata, ISupportHasChanges, IEquatable(Of IComposer), IEquatable(Of String)

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property Aliases As BindingList(Of String)
        Get
            Return _Aliases
        End Get
        Set(value As BindingList(Of String))
            If value IsNot _Aliases Then
                HasChanges = True
                _Aliases = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Aliases)))
            End If
        End Set
    End Property
    Private WithEvents _Aliases As New BindingList(Of String)

    Property PrimaryName As String Implements IComposer.Composer
        Get
            Return _PrimaryName
        End Get
        Set(value As String)
            If Not _PrimaryName = value Then
                HasChanges = True
                _PrimaryName = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PrimaryName)))
            End If
        End Set
    End Property
    Private _PrimaryName As String

    Property Era As Era Implements IComposerMetadata.Era
        Get
            Return _Era
        End Get
        Set(value As Era)
            If Not _Era Is value Then
                HasChanges = True
                _Era = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Era)))
            End If
        End Set
    End Property
    Private _Era As Era

    Property MetadataId As Integer Implements IComposerMetadata.MetadataId
        Get
            Return _MetadataId
        End Get
        Set(value As Integer)
            If Not _MetadataId = value Then
                HasChanges = True
                _MetadataId = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(MetadataId)))
            End If
        End Set
    End Property
    Private _MetadataId As Integer = -1

    <Editable(False)>
    <Display(AutoGenerateField:=False)>
    Public Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            Return _HasChanges
        End Get
        Set(value As Boolean)
            _HasChanges = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean = True

    Sub New()

    End Sub

    Sub New(primaryName As String, aliases As IEnumerable(Of String))
        Me.PrimaryName = primaryName

        Dim aliasList = aliases.ToList
        aliasList.Sort()
        For Each aliasName In aliasList
            Me.Aliases.Add(aliasName)
        Next
    End Sub

    Friend Sub MergeFrom(rec As IComposerAlternates)
        Dim aliasList As New List(Of String)
        aliasList.Add(Me.PrimaryName)
        aliasList.AddRange(Me.Aliases)
        aliasList.Add(rec.Composer)
        aliasList.AddRange(rec.AlternateComposerSpellings)
        aliasList = aliasList.Distinct.ToList
        aliasList.Remove(Me.PrimaryName)
        aliasList.Sort()

        Me.Aliases.Clear()
        For Each aliasName In aliasList
            Me.Aliases.Add(aliasName)
        Next
    End Sub

    Public Overrides Function ToString() As String
        Return PrimaryName
    End Function

    Private Sub OnAliasesListChanged(sender As Object, e As ListChangedEventArgs) Handles _Aliases.ListChanged
        HasChanges = True
    End Sub

    Public Shadows Function Equals(other As IComposer) As Boolean Implements IEquatable(Of IComposer).Equals
        Return Equals(other.Composer)
    End Function


    Public Shadows Function Equals(other As String) As Boolean Implements IEquatable(Of String).Equals
        Dim otherName = If(other = Nothing, String.Empty, other.ToUpper.Trim)
        Dim meName = If(Me.PrimaryName = Nothing, String.Empty, Me.PrimaryName.ToUpper.Trim)
        Dim meNameNoComma = FormatNameWithNoCommas(meName)


        If otherName = meName Then Return True
        If otherName = meNameNoComma Then Return True

        If Me.Aliases.Select(Function(y) y.ToUpper.Trim).Contains(otherName) Then Return True
        If Me.Aliases.Select(Function(y) FormatNameWithNoCommas(y.ToUpper.Trim)).Contains(otherName) Then Return True

        Return False
    End Function
End Class
