import type { ChannelDto } from "../api/dtos";

const VoiceChannel: React.FC<ChannelDto> = ({ id, serverId, name, channelType }) => {
  return (
      <p>{ name }</p>
  );
}

export default VoiceChannel;