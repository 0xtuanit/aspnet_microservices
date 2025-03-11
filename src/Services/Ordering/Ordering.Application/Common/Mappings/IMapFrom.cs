using AutoMapper;

namespace Ordering.Application.Common.Mappings;

public interface IMapFrom<T>
{
    // Remember the Mapping name below
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}