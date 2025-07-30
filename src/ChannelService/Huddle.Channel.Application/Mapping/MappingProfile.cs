using AutoMapper;
using Huddle.Channel.Domain.Aggregates.InviteAggregate;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;
using Huddle.Channel.Domain.Aggregates.MessageAggregate;
using Huddle.Channel.Domain.Aggregates.ServerAggregate;

namespace Huddle.Channel.Application.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Message, MessageDto>();
            CreateMap<Server, ServerDto>();
            CreateMap<Member, MemberDto>();
            CreateMap<Invite, InviteDto>();

            CreateMap<Domain.Aggregates.ServerAggregate.Channel, ChannelDto>()
                .ForCtorParam("ChannelType", opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
