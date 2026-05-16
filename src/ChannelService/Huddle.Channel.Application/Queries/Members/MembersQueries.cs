using AutoMapper;
using Huddle.Channel.Application.Dto;
using Huddle.Channel.Domain;
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

        public async Task<PaginatedItems<MemberDto>> GetByServerId(Guid serverId, Guid? cursor = null, int limit = 50)
        {
            var paginatedMembers = await _memberRepository.GetByServerIdAsync(serverId, cursor, limit);

            var membersDto = paginatedMembers.Items.Select(_mapper.Map<MemberDto>).ToList();

            return new PaginatedItems<MemberDto>(
                membersDto,
                false,
                paginatedMembers.HasNext,
                paginatedMembers.NextCursor,
                null
            );
        }
    }
}
