using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    /// <summary>
    /// Dropdown listing concrete, non-Unity-Object types derived from a base type, grouped by namespace, for
    /// selecting a type to instantiate into a polymorphic (<c>SerializeReference</c>) field.
    /// </summary>
    public class PolymorphicTypeSelectorDropdown : AdvancedDropdown
    {
        /// <summary>
        /// Raised when the user selects an enabled type from the dropdown.
        /// </summary>
        public event Action<Type> ComponentTypeSelected;

        private readonly Type _baseType;

        private readonly IEnumerable<Type> _disabledTypes;

        private readonly List<SelectableType> _selectableTypes = new();

        private readonly Dictionary<int, SelectableType> _itemIdToSelectableType = new();

        /// <summary>
        /// Creates a dropdown listing types derived from <paramref name="baseType"/>.
        /// </summary>
        /// <param name="baseType">The base type whose derived types are listed.</param>
        public PolymorphicTypeSelectorDropdown(Type baseType)
            : this(baseType, Enumerable.Empty<Type>(), new AdvancedDropdownState()) { }

        /// <summary>
        /// Creates a dropdown listing types derived from <paramref name="baseType"/>, with some types shown
        /// disabled.
        /// </summary>
        /// <param name="baseType">The base type whose derived types are listed.</param>
        /// <param name="disabledTypes">Types to display as disabled/non-selectable.</param>
        public PolymorphicTypeSelectorDropdown(Type baseType, IEnumerable<Type> disabledTypes)
            : this(baseType, disabledTypes, new AdvancedDropdownState()) { }

        /// <summary>
        /// Creates a dropdown listing types derived from <paramref name="baseType"/>, using an existing dropdown
        /// state.
        /// </summary>
        /// <param name="baseType">The base type whose derived types are listed.</param>
        /// <param name="state">The persisted state to use for the dropdown.</param>
        public PolymorphicTypeSelectorDropdown(Type baseType, AdvancedDropdownState state) :
            this(baseType, Enumerable.Empty<Type>(), state) { }

        /// <summary>
        /// Creates a dropdown listing types derived from <paramref name="baseType"/>, with some types shown
        /// disabled, using an existing dropdown state.
        /// </summary>
        /// <param name="baseType">The base type whose derived types are listed.</param>
        /// <param name="disabledTypes">Types to display as disabled/non-selectable.</param>
        /// <param name="state">The persisted state to use for the dropdown.</param>
        public PolymorphicTypeSelectorDropdown(
            Type baseType,
            IEnumerable<Type> disabledTypes,
            AdvancedDropdownState state) : base(state)
        {
            _baseType = baseType;
            _disabledTypes = disabledTypes;
        }

        private static bool IsValidType(Type t)
        {
            return !t.IsAbstract && !t.IsInterface && !typeof(UnityEngine.Object).IsAssignableFrom(t);
        }

        /// <inheritdoc />
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            SelectableType type = _itemIdToSelectableType[item.id];
            if (!type.IsEnabled)
            {
                EditorUtility.DisplayDialog(
                    "Invalid Operation",
                    $"Only one '{type.Type.FullName}' is allowed to be added.",
                    "OK");
                return;
            }

            ComponentTypeSelected?.Invoke(type.Type);
        }

        /// <inheritdoc />
        protected override AdvancedDropdownItem BuildRoot()
        {
            TypeCache.TypeCollection allDerivedTypes = TypeCache.GetTypesDerivedFrom(_baseType);
            foreach (Type type in allDerivedTypes)
            {
                if (!IsValidType(type))
                {
                    continue;
                }

                bool enabled = !_disabledTypes.Contains(type);
                _selectableTypes.Add(new SelectableType(type, enabled));
            }

            _selectableTypes.Sort();

            var root = new AdvancedDropdownItem($"{_baseType.Name}")
            {
                id = 0,
            };
            IEnumerable<IGrouping<string, SelectableType>> selectableTypesByNamespace = _selectableTypes
                .Where(t => t.Type.Namespace.IsNotNull())
                .GroupBy(t => t.Type.Namespace)
                .OrderBy(g => g.Key);
            GUIContent scriptIconContent = EditorGUIUtility.IconContent("cs Script Icon");
            var scriptIcon = (Texture2D)scriptIconContent.image;
            foreach (IGrouping<string, SelectableType> group in selectableTypesByNamespace)
            {
                var nsItem = new AdvancedDropdownItem(group.Key)
                {
                    enabled = true,
                };
                root.AddChild(nsItem);

                foreach (SelectableType selectableType in group)
                {
                    var typeItem = new AdvancedDropdownItem(selectableType.Type.Name)
                    {
                        enabled = selectableType.IsEnabled,
                        icon = scriptIcon,
                    };
                    nsItem.AddChild(typeItem);
                    _itemIdToSelectableType.Add(typeItem.id, selectableType);
                }
            }

            foreach (SelectableType selectableType in _selectableTypes.Where(t => t.Type.Namespace.IsNull()))
            {
                var typeItem = new AdvancedDropdownItem(selectableType.Type.Name)
                {
                    enabled = selectableType.IsEnabled,
                    icon = scriptIcon,
                };
                root.AddChild(typeItem);
                _itemIdToSelectableType.Add(typeItem.id, selectableType);
            }

            return root;
        }

        private readonly struct SelectableType : IComparable<SelectableType>, IComparable
        {
            public readonly Type Type;

            public readonly bool IsEnabled;

            public SelectableType(Type type, bool isEnabled)
            {
                Type = type;
                IsEnabled = isEnabled;
            }

            public int CompareTo(SelectableType other)
            {
                return string.Compare(Type.FullName, other.Type.FullName, StringComparison.OrdinalIgnoreCase);
            }

            public int CompareTo(object obj)
            {
                if (obj is null)
                {
                    return 1;
                }

                return obj is SelectableType other
                    ? CompareTo(other)
                    : throw new ArgumentException($"Object must be of type {nameof(SelectableType)}");
            }

            public static bool operator <(SelectableType left, SelectableType right)
            {
                return left.CompareTo(right) < 0;
            }

            public static bool operator >(SelectableType left, SelectableType right)
            {
                return left.CompareTo(right) > 0;
            }

            public static bool operator <=(SelectableType left, SelectableType right)
            {
                return left.CompareTo(right) <= 0;
            }

            public static bool operator >=(SelectableType left, SelectableType right)
            {
                return left.CompareTo(right) >= 0;
            }
        }
    }
}