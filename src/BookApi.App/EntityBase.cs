﻿// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See LICENSE in the project root for license information.

namespace BookApi.App
{
  /// <summary>Represents an entity base.</summary>
  public abstract class EntityBase : IUpdatable<object>
  {
    private readonly IList<string> _properties;

    /// <summary>Initializes a new instance of the <see cref="EntityBase"/> class.</summary>
    protected EntityBase()
    {
      _properties = new List<string>();
    }

    /// <summary>Gets/sets an object that represents a collection of updated properties.</summary>
    public IEnumerable<string> Properties => _properties;

    /// <summary>Updates this entity.</summary>
    /// <param name="newEntity">An object that represents an entity from which this entity should be updated.</param>
    public void Update(object newEntity)
    {
      var updatingProperties = GetUpdatingProperties();
      var updatedProperties = updatingProperties;

      Update(newEntity, updatedProperties, updatingProperties);
    }

    /// <summary>Updates this entity.</summary>
    /// <param name="newEntity">An object that represents an entity from which this entity should be updated.</param>
    /// <param name="properties">An object that represents a collection of properties to update.</param>
    public void Update(object newEntity, IEnumerable<string> properties) =>
      Update(newEntity, properties, GetUpdatingProperties());

    protected virtual void Update(object newEntity, IEnumerable<string> updatedProperties, ISet<string> updatingProperties)
    {
      foreach (var property in updatedProperties)
      {
        if (updatingProperties.Contains(property))
        {
          var originalProperty = GetType().GetProperty(property)!;
          var newProperty = newEntity.GetType().GetProperty(property)!;

          var originalValue = originalProperty.GetValue(this);
          var newValue = newProperty.GetValue(newEntity);

          if (!object.Equals(originalValue, newValue))
          {
            originalProperty.SetValue(this, newValue);
          }

          _properties.Add(property);
        }
      }
    }

    /// <summary>Creates a copy of an entity.</summary>
    /// <param name="entity">An object that represents an entity to copy.</param>
    /// <returns>An object that represents an instance of an entity copy.</returns>
    /// <exception cref="System.NotSupportedException">Throws if there is no such entity.</exception>
    public static T2 Create<T1, T2>(T1 entity) where T2 : EntityBase, T1
    {
      ArgumentNullException.ThrowIfNull(entity);

      if (entity.GetType() == typeof(T2))
      {
        return (T2)entity;
      }

      return (T2)typeof(T2).GetConstructor(new[] { typeof(T1) })!
                           .Invoke(new object[] { entity! });
    }

    private ISet<string> GetUpdatingProperties() =>
      GetType().GetProperties()
               .Where(property => property.CanWrite)
               .Select(property => property.Name)
               .ToHashSet(StringComparer.OrdinalIgnoreCase);
  }
}