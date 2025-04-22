Imports System.Runtime.CompilerServices

Namespace Extensions.ObjectExtensions
    <HideModuleName>
    Public Module M_ObjectExtensions

        Private _ObjectExtensions As New ObjectExtensions

        ''' <summary>
        ''' Returns true if <paramref name="value"/> is <see cref="DBNull.Value"/>, <see langword="Nothing"/>, or <see cref="String.Empty"/>.
        ''' </summary>
        ''' <param name="value">The value to check.</param>
        <Extension>
        Public Function IsDbNullNothingEmpty(value As Object) As Boolean
            Return _ObjectExtensions.IsDbNullNothingEmpty(value)
        End Function

        ''' <summary>
        ''' Returns true if <paramref name="value"/> is <see cref="DBNull.Value"/>, <see langword="Nothing"/>, or <see cref="String.Empty"/>.
        ''' </summary>
        ''' <param name="value">The value to check.</param>
        ''' <param name="trimStrings">When true, strings that are whitespace will be considered empty.</param>
        <Extension>
        Public Function IsDbNullNothingEmpty(value As Object, trimStrings As Boolean) As Boolean
            Return _ObjectExtensions.IsDbNullNothingEmpty(value, trimStrings)
        End Function

        ''' <summary>
        ''' Returns true if <paramref name="value"/> is <see cref="DBNull.Value"/>, <see langword="Nothing"/>, <see cref="String.Empty"/>, or
        ''' if the value is numeric and equals "0".
        ''' </summary>
        ''' <param name="value">The value to check.</param>
        <Extension>
        Public Function IsDbNullNothingEmptyZero(value As Object) As Boolean
            Return _ObjectExtensions.IsDbNullNothingEmptyZero(value)
        End Function

        ''' <summary>
        ''' Returns true if <paramref name="value"/> is <see cref="DBNull.Value"/>, <see langword="Nothing"/>, <see cref="String.Empty"/>, or
        ''' if the value is numeric and equals "0".
        ''' </summary>
        ''' <param name="value">The value to check.</param>
        ''' <param name="trimStrings">When true, strings that are whitespace will be considered empty.</param>
        <Extension>
        Public Function IsDbNullNothingEmptyZero(value As Object, trimStrings As Boolean) As Boolean
            Return _ObjectExtensions.IsDbNullNothingEmptyZero(value, trimStrings)
        End Function

        ''' <summary>
        ''' Adds a way to access the Count of an IEnumerable.
        ''' </summary>
        <Extension>
        Public Function GetCount(ByRef collection As IEnumerable) As Integer
            Return _ObjectExtensions.GetCount(collection)
        End Function

        ''' <summary>
        ''' Returns the generic types from a type's interface(s).
        ''' </summary>
        ''' <param name="genericObjectType">The type whose interfaces we are checking.</param>
        ''' <param name="mustImplementInterfaces">The interfaces that the generic type must implement. 
        ''' For example, in "Implements IList(Of T)", T must implement these interfaces.</param>
        ''' <returns></returns>
        <Extension>
        Public Function GetGenericTypesFromInterfaces(ByRef genericObjectType As Type, mustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypesFromInterfaces(genericObjectType, mustImplementInterfaces)
        End Function

        ''' <summary>
        ''' Returns the generic types from a type's interface(s).
        ''' </summary>
        ''' <param name="genericObjectType">The type whose interfaces we are checking.</param>
        ''' <param name="mustImplementInterfaces">The interfaces that the generic type must implement. 
        ''' For example, in "Implements IList(Of T)", T must implement these interfaces.</param>
        ''' <returns></returns>
        <Extension>
        Public Function GetGenericTypesFromInterfaces(ByRef genericObjectType As Type, mustImplementInterfaces As IEnumerable(Of Type), genericInterfaceMustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypesFromInterfaces(genericObjectType, mustImplementInterfaces, genericInterfaceMustImplementInterfaces)
        End Function

        <Extension>
        Public Function GetGenericTypesFromInterfaces(ByRef genericObjectType As Type, assemblyNameIncludes As IEnumerable(Of String)) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypesFromInterfaces(genericObjectType, assemblyNameIncludes)
        End Function

        <Extension>
        Public Function GetGenericTypesFromInterfaces(ByRef genericObjectType As Type, assemblyOfGenericType As Reflection.Assembly) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypesFromInterfaces(genericObjectType, assemblyOfGenericType)
        End Function

        <Extension>
        Public Function GetGenericTypesFromInterfaces(ByRef genericObjectType As Type) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypesFromInterfaces(genericObjectType)
        End Function

        <Extension>
        Public Function GetGenericTypesFromInterfaces(ByRef genericObject As Object, mustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypesFromInterfaces(genericObject, mustImplementInterfaces)
        End Function

        <Extension>
        Public Function GetGenericTypesFromInterfaces(ByRef genericObject As Object, mustImplementInterfaces As IEnumerable(Of Type), genericInterfaceMustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypesFromInterfaces(genericObject, mustImplementInterfaces, genericInterfaceMustImplementInterfaces)
        End Function

        <Extension>
        Public Function GetGenericTypesFromInterfaces(ByRef genericObject As Object, assemblyNameIncludes As IEnumerable(Of String)) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypesFromInterfaces(genericObject, assemblyNameIncludes)
        End Function

        <Extension>
        Public Function GetGenericTypesFromInterfaces(ByRef genericObject As Object, assemblyOfGenericType As Reflection.Assembly) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypesFromInterfaces(genericObject, assemblyOfGenericType)
        End Function

        <Extension>
        Public Function GetGenericTypesFromInterfaces(ByRef genericObject As Object) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypesFromInterfaces(genericObject)
        End Function

        <Extension>
        Public Function GetGenericTypes(ByRef genericObjectType As Type, mustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypes(genericObjectType, mustImplementInterfaces)
        End Function

        <Extension>
        Public Function GetGenericTypes(ByRef genericObjectType As Type, assemblyNameIncludes As IEnumerable(Of String)) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypes(genericObjectType, assemblyNameIncludes)
        End Function

        <Extension>
        Public Function GetGenericTypes(ByRef genericObjectType As Type, assemblyOfGenericType As Reflection.Assembly) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypes(genericObjectType, assemblyOfGenericType)
        End Function

        <Extension>
        Public Function GetGenericTypes(ByRef genericObject As Object, mustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypes(genericObject, mustImplementInterfaces)
        End Function

        <Extension>
        Public Function GetGenericTypes(ByRef genericObject As Object, assemblyNameIncludes As IEnumerable(Of String)) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypes(genericObject, assemblyNameIncludes)
        End Function

        <Extension>
        Public Function GetGenericTypes(ByRef genericObject As Object, assemblyOfGenericType As Reflection.Assembly) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypes(genericObject, assemblyOfGenericType)
        End Function

        <Extension>
        Public Function GetGenericTypes(ByRef genericObject As Object) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypes(genericObject)
        End Function

        <Extension>
        Public Function GetGenericTypes(ByRef genericObjectType As Type) As List(Of Type)
            Return _ObjectExtensions.GetGenericTypes(genericObjectType)
        End Function

        ''' <summary>
        ''' Returns true if the given type, and of its property types (recursive), or any of its collection type item 
        ''' types are inherited from <see cref="GSSEO.EOSharedCommon"/>. Returns False otherwise.
        ''' </summary>
        ''' <param name="typeOfData">The type of data to check.</param>
        <Extension>
        Public Function TypeContainsPropertyOfType(ByRef typeOfData As Type, typesToFind As IEnumerable(Of Type)) As Boolean
            Return _ObjectExtensions.TypeContainsPropertyOfType(typeOfData, typesToFind)
        End Function

    End Module

    Public Class ObjectExtensions
        ''' <summary>
        ''' Returns true if <paramref name="value"/> is <see cref="DBNull.Value"/>, <see langword="Nothing"/>, or <see cref="String.Empty"/>.
        ''' </summary>
        ''' <param name="value">The value to check.</param>
        Overridable Function IsDbNullNothingEmpty(value As Object) As Boolean
            Return IsDbNullNothingEmpty(value, False)
        End Function

        ''' <summary>
        ''' Returns true if <paramref name="value"/> is <see cref="DBNull.Value"/>, <see langword="Nothing"/>, or <see cref="String.Empty"/>.
        ''' </summary>
        ''' <param name="value">The value to check.</param>
        ''' <param name="trimStrings">When true, strings that are whitespace will be considered empty.</param>
        Overridable Function IsDbNullNothingEmpty(value As Object, trimStrings As Boolean) As Boolean
            ' Perform basic checks for known types.
            If (value Is Nothing OrElse value Is DBNull.Value OrElse
                (TypeOf value Is String AndAlso If(trimStrings, value.Trim, value) = String.Empty) OrElse
                (TypeOf value Is Date AndAlso value = Date.MinValue)) Then
                Return True
            End If

            ' Value is not nothing here. If it's a primitive type, it's NOT nothing/empty.
            If value.GetType.IsPrimitive Then
                Return False
            End If

            ' We've already tested "is nothing", so it's an item. However, some
            ' types can equal nothing even if they are an object. For example, 
            ' a color set to nothing actually equals (0, 0, 0, 0) with IsEmpty as
            ' true, but should return "True" for a nothing test.
            '
            ' So we see if it implements the equals sign, and if it does, we
            ' return whether or not it "= Nothing". If it doesn't implement the 
            ' equals sign, we've already tested nothing-ness, so it's not nothing.
            If value.GetType.GetMethod("op_Equality") IsNot Nothing Then
                Return value = Nothing
            Else
                Return False
            End If

        End Function

        ''' <summary>
        ''' Returns true if <paramref name="value"/> is <see cref="DBNull.Value"/>, <see langword="Nothing"/>, <see cref="String.Empty"/>, or
        ''' if the value is numeric and equals "0".
        ''' </summary>
        ''' <param name="value">The value to check.</param>
        Overridable Function IsDbNullNothingEmptyZero(value As Object) As Boolean
            Return IsDbNullNothingEmptyZero(value, False)
        End Function

        ''' <summary>
        ''' Returns true if <paramref name="value"/> is <see cref="DBNull.Value"/>, <see langword="Nothing"/>, <see cref="String.Empty"/>, or
        ''' if the value is numeric and equals "0".
        ''' </summary>
        ''' <param name="value">The value to check.</param>
        ''' <param name="trimStrings">When true, strings that are whitespace will be considered empty.</param>
        Overridable Function IsDbNullNothingEmptyZero(value As Object, trimStrings As Boolean) As Boolean
            ' First we use the above function to check DbNull/Nothing/Empty.
            If IsDbNullNothingEmpty(value, trimStrings) Then Return True

            ' If it's not, then we check if it's numeric and equal to 0.
            If IsNumeric(value) AndAlso value = 0 Then Return True

            ' If it's not 0, return false.
            Return False

        End Function

        Overridable Function GetDescriptionAttributeText(attributeType As Type, type As Type, Optional memberName As String = Nothing) As String
            Dim returnString As String = Nothing
            If TryGetDescriptionAttributeText(returnString, attributeType, type, memberName) Then
                Return returnString
            End If
            Return Nothing
        End Function

        Overridable Function TryGetDescriptionAttributeText(ByRef returnString As String, attributeType As Type, type As Type, Optional memberName As String = Nothing) As Boolean

            If memberName = Nothing Then
                Dim translation As ComponentModel.DescriptionAttribute = type.GetCustomAttributes(attributeType, False).FirstOrDefault
                If translation IsNot Nothing Then
                    returnString = translation.Description
                    Return True
                End If
                returnString = type.Name
            Else
                Dim memberInfo = type.GetMember(memberName).FirstOrDefault
                If memberInfo IsNot Nothing Then
                    Dim translation As ComponentModel.DescriptionAttribute = Attribute.GetCustomAttributes(memberInfo, attributeType, True).FirstOrDefault
                    If translation IsNot Nothing Then
                        returnString = translation.Description
                        Return True
                    End If
                End If
                returnString = memberName
            End If

            Return False

        End Function

        Overridable Function GetDisplayNameAttributeText(attributeType As Type, type As Type, Optional memberName As String = Nothing) As String
            Dim returnString As String = Nothing
            If TryGetDisplayNameAttributeText(returnString, attributeType, type, memberName) Then
                Return returnString
            End If
            Return Nothing
        End Function

        Overridable Function TryGetDisplayNameAttributeText(ByRef returnString As String, attributeType As Type, type As Type, Optional memberName As String = Nothing) As Boolean

            If memberName = Nothing Then
                Dim translation As ComponentModel.DisplayNameAttribute = type.GetCustomAttributes(attributeType, False).FirstOrDefault
                If translation IsNot Nothing Then
                    returnString = translation.DisplayName
                    Return True
                End If
                returnString = type.Name
            Else
                Dim memberInfo = type.GetMember(memberName).FirstOrDefault
                If memberInfo IsNot Nothing Then
                    Dim translation As ComponentModel.DisplayNameAttribute = Attribute.GetCustomAttributes(memberInfo, attributeType, True).FirstOrDefault
                    If translation IsNot Nothing Then
                        returnString = translation.DisplayName
                        Return True
                    End If
                End If
                returnString = memberName
            End If

            Return False

        End Function

        ''' <summary>
        ''' Adds a way to access the Count of an IEnumerable.
        ''' </summary>
        Overridable Function GetCount(ByRef collection As IEnumerable) As Integer
            If collection Is Nothing Then Return 0
            If TypeOf collection Is ICollection Then Return DirectCast(collection, ICollection).Count

            Dim itemCount As Integer = 0
            For Each item In collection
                itemCount += 1
            Next
            Return itemCount
        End Function

        ''' <summary>
        ''' Returns the generic types from a type's interface(s).
        ''' </summary>
        ''' <param name="genericObjectType">The type whose interfaces we are checking.</param>
        ''' <param name="mustImplementInterfaces">The interfaces that the generic type must implement. 
        ''' For example, in "Implements IList(Of T)", T must implement these interfaces.</param>
        ''' <returns></returns>
        Overridable Function GetGenericTypesFromInterfaces(ByRef genericObjectType As Type, mustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            Return GetGenericTypesFromInterfaces(genericObjectType, mustImplementInterfaces, New List(Of Type))
        End Function

        ''' <summary>
        ''' Returns the generic types from a type's interface(s).
        ''' </summary>
        ''' <param name="genericObjectType">The type whose interfaces we are checking.</param>
        ''' <param name="mustImplementInterfaces">The interfaces that the generic type must implement. 
        ''' For example, in "Implements IList(Of T)", T must implement these interfaces.</param>
        ''' <returns></returns>
        Overridable Function GetGenericTypesFromInterfaces(ByRef genericObjectType As Type, mustImplementInterfaces As IEnumerable(Of Type), genericInterfaceMustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)

            Dim type As Type = genericObjectType

            Dim foundTypes As New List(Of Type)
            ' We check the type's interfaces for generic types like "IList(Of T)". The interface can give us the type
            ' of data the collection can hold even if Count is 0.
            For Each interfaceType In type.GetInterfaces
                GetGenericTypesInternal(interfaceType, genericInterfaceMustImplementInterfaces, foundTypes)
            Next

            Return foundTypes.FindAll(Function(item)
                                          For Each mustImplementInterface In mustImplementInterfaces
                                              If item.GetInterface(mustImplementInterface.FullName) Is Nothing Then
                                                  Return False
                                              End If
                                          Next
                                          Return True
                                      End Function)

        End Function

        Overridable Function GetGenericTypesFromInterfaces(ByRef genericObjectType As Type, assemblyNameIncludes As IEnumerable(Of String)) As List(Of Type)
            Return GetGenericTypesFromInterfaces(genericObjectType, New List(Of Type), New List(Of Type)).FindAll(Function(item)
                                                                                                                      Dim hasAllParts As Boolean = True
                                                                                                                      For Each assemblyNamePart As String In assemblyNameIncludes
                                                                                                                          If Not item.FullName.ToLower.Contains(assemblyNamePart.ToLower) Then
                                                                                                                              hasAllParts = False
                                                                                                                          End If
                                                                                                                      Next
                                                                                                                      Return hasAllParts
                                                                                                                  End Function)
        End Function

        Overridable Function GetGenericTypesFromInterfaces(ByRef genericObjectType As Type, assemblyOfGenericType As Reflection.Assembly) As List(Of Type)

            Return GetGenericTypesFromInterfaces(genericObjectType, New List(Of Type), New List(Of Type)).FindAll(Function(item)
                                                                                                                      Return item.Assembly.FullName = assemblyOfGenericType.FullName
                                                                                                                  End Function)
        End Function

        Overridable Function GetGenericTypesFromInterfaces(ByRef genericObjectType As Type) As List(Of Type)
            Return GetGenericTypesFromInterfaces(genericObjectType, New List(Of Type), New List(Of Type))
        End Function

        Overridable Function GetGenericTypesFromInterfaces(ByRef genericObject As Object, mustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            If genericObject Is Nothing Then Return New List(Of Type)
            Return GetGenericTypesFromInterfaces(genericObject, mustImplementInterfaces, New List(Of Type))
        End Function

        Overridable Function GetGenericTypesFromInterfaces(ByRef genericObject As Object, mustImplementInterfaces As IEnumerable(Of Type), genericInterfaceMustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            Return GetGenericTypesFromInterfaces(genericObject.GetType, mustImplementInterfaces, genericInterfaceMustImplementInterfaces)
        End Function

        Overridable Function GetGenericTypesFromInterfaces(ByRef genericObject As Object, assemblyNameIncludes As IEnumerable(Of String)) As List(Of Type)
            If genericObject Is Nothing Then Return New List(Of Type)
            Return GetGenericTypesFromInterfaces(genericObject.GetType, assemblyNameIncludes)
        End Function

        Overridable Function GetGenericTypesFromInterfaces(ByRef genericObject As Object, assemblyOfGenericType As Reflection.Assembly) As List(Of Type)
            If genericObject Is Nothing Then Return New List(Of Type)
            Return GetGenericTypesFromInterfaces(genericObject.GetType, assemblyOfGenericType)
        End Function

        Overridable Function GetGenericTypesFromInterfaces(ByRef genericObject As Object) As List(Of Type)
            If genericObject Is Nothing Then Return Nothing
            Return GetGenericTypesFromInterfaces(genericObject.GetType)
        End Function

        Overridable Function GetGenericTypes(ByRef genericObjectType As Type, mustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            Return GetGenericTypes(genericObjectType).FindAll(Function(item)
                                                                  For Each mustImplementInterface In mustImplementInterfaces
                                                                      If item.GetInterface(mustImplementInterface.FullName) Is Nothing Then
                                                                          Return False
                                                                      End If
                                                                  Next
                                                                  Return True
                                                              End Function)
        End Function

        Overridable Function GetGenericTypes(ByRef genericObjectType As Type, assemblyNameIncludes As IEnumerable(Of String)) As List(Of Type)
            Return GetGenericTypes(genericObjectType).FindAll(Function(item)
                                                                  Dim hasAllParts As Boolean = True
                                                                  For Each assemblyNamePart As String In assemblyNameIncludes
                                                                      If Not item.FullName.ToLower.Contains(assemblyNamePart.ToLower) Then
                                                                          hasAllParts = False
                                                                      End If
                                                                  Next
                                                                  Return hasAllParts
                                                              End Function)
        End Function

        Overridable Function GetGenericTypes(ByRef genericObjectType As Type, assemblyOfGenericType As Reflection.Assembly) As List(Of Type)

            Return GetGenericTypes(genericObjectType).FindAll(Function(item)
                                                                  Return item.Assembly.FullName = assemblyOfGenericType.FullName
                                                              End Function)
        End Function

        Overridable Function GetGenericTypes(ByRef genericObject As Object, mustImplementInterfaces As IEnumerable(Of Type)) As List(Of Type)
            If genericObject Is Nothing Then Return New List(Of Type)
            Return GetGenericTypes(genericObject.GetType, mustImplementInterfaces)
        End Function

        Overridable Function GetGenericTypes(ByRef genericObject As Object, assemblyNameIncludes As IEnumerable(Of String)) As List(Of Type)
            If genericObject Is Nothing Then Return New List(Of Type)
            Return GetGenericTypes(genericObject.GetType, assemblyNameIncludes)
        End Function

        Overridable Function GetGenericTypes(ByRef genericObject As Object, assemblyOfGenericType As Reflection.Assembly) As List(Of Type)
            If genericObject Is Nothing Then Return New List(Of Type)
            Return GetGenericTypes(genericObject.GetType, assemblyOfGenericType)
        End Function

        Overridable Function GetGenericTypes(ByRef genericObject As Object) As List(Of Type)
            If genericObject Is Nothing Then Return Nothing
            Return GetGenericTypes(genericObject.GetType)
        End Function

        Overridable Function GetGenericTypes(ByRef genericObjectType As Type) As List(Of Type)
            Dim foundTypes As New List(Of Type)
            GetGenericTypesInternal(genericObjectType, New List(Of Type), foundTypes)
            Return foundTypes
        End Function

        Overridable Sub GetGenericTypesInternal(ByRef genericObjectType As Type, genericTypeMustImplementInterfaces As IEnumerable(Of Type), ByRef foundTypes As List(Of Type))
            ' We check the type for generic types like "(Of T)". The interface can give us the type
            ' of data the collection can hold even if Count is 0.
            If genericObjectType.IsGenericType Then
                For Each interfaceType As Type In genericTypeMustImplementInterfaces
                    If Not interfaceType.IsAssignableFrom(genericObjectType) Then
                        Exit Sub
                    End If
                Next

                For Each genericArgument In genericObjectType.GenericTypeArguments
                    If Not foundTypes.Contains(genericArgument) Then
                        foundTypes.Add(genericArgument)
                    End If
                Next
            End If
        End Sub


        ''' <summary>
        ''' Returns true if the given type, and of its property types (recursive), or any of its collection type item 
        ''' types are inherited from <see cref="GSSEO.EOSharedCommon"/>. Returns False otherwise.
        ''' </summary>
        ''' <param name="typeOfData">The type of data to check.</param>
        Overridable Function TypeContainsPropertyOfType(ByRef typeOfData As Type, typesToFind As IEnumerable(Of Type)) As Boolean
            Dim typesToCheck As New Queue(Of Type)
            Dim typesChecked As New List(Of Type)
            typesToCheck.Enqueue(typeOfData)

            While typesToCheck.Count > 0
                Dim typeToCheck = typesToCheck.Dequeue
                For Each typeToFind In typesToFind
                    If typeToFind.IsAssignableFrom(typeToCheck) Then Return True
                Next

                If typeToCheck.IsPrimitive OrElse
                        typeToCheck.IsEnum OrElse
                        typeToCheck = GetType(String) OrElse
                        typeToCheck = GetType(DateTime) OrElse
                        typeToCheck = GetType(TimeSpan) Then

                    ' Do nothing, these are primitive types (or at least have no sub-classes).
                ElseIf GetType(IEnumerable).IsAssignableFrom(typeToCheck) Then
                    ' Is a collection type. Need to find what the collection item type is and add that.
                    ' We can only reasonable do this if it has a generic type, otherwise, we do not know what
                    ' the item type is without loading the data.
                    For Each genericType In GetGenericTypesFromInterfaces(typeToCheck)
                        If Not typesChecked.Contains(genericType) Then
                            typesToCheck.Enqueue(genericType)
                            typesChecked.Add(genericType)
                        End If
                    Next
                Else
                    ' Get all the property types and add them to the queue
                    For Each propertyInfo In typeToCheck.GetProperties()
                        If Not typesChecked.Contains(propertyInfo.PropertyType) Then
                            typesToCheck.Enqueue(propertyInfo.PropertyType)
                            typesChecked.Add(propertyInfo.PropertyType)
                        End If
                    Next
                End If
            End While

            Return False
        End Function

    End Class
End Namespace