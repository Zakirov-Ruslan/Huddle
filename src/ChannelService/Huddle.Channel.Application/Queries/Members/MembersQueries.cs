using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain.Aggregates.MemberAggregate;

namespace Huddle.Channel.Application.Queries.Members
{
    public class MembersQueries : IMembersQueries
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;

        public MembersQueries(IMemberRepository memberRepository, IMapper mapper)
        {
            _memberRepository = memberRepository;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetAsync(Guid memberId)
        {
            var member = await _memberRepository.GetAsync(memberId);
            return _mapper.Map<MemberDto>(member);
        }

        public async Task<IEnumerable<MemberDto>> GetByServerId(Guid serverId)
        {
            var members = await _memberRepository.GetByServerIdAsync(serverId);

            return members.Select(_mapper.Map<MemberDto>);
        }
    }
}
