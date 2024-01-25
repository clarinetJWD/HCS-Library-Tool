Imports System.ComponentModel

Public Interface ISupportHasChanges
    Property HasChanges As Boolean
End Interface

Public Interface IName
    Property Name As String
End Interface

Public Interface IId
    Property Id As String
End Interface

Public Interface INameId : Inherits IName, IId

End Interface

Public Interface IMetadata
    Property Length As TimeSpan
    Property Difficulty As Difficulties
    Property Tags As Tags
    Property Era As Era
End Interface

Public Interface IComposer : Inherits INotifyPropertyChanged
    Property Composer As String

End Interface

Public Interface IArranger : Inherits INotifyPropertyChanged
    Property Arranger As String

End Interface

Public Interface ITitle : Inherits INotifyPropertyChanged
    Property Title As String

End Interface

Public Interface IComposerTitle : Inherits IComposer, ITitle

End Interface

Public Interface IComposerArrangerTitle : Inherits IComposerTitle, IArranger

End Interface

Public Interface IComposerAlternates : Inherits IComposer
    Property AlternateComposerSpellings As BindingList(Of String)
End Interface