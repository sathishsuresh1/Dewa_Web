﻿namespace DEWAXP.Feature.FormsExtensions.ValueProviders
{
    public interface IFieldValueBinderResult
    {
        bool HasValue();
        object Value { get; }
    }
}