import Mascot from "../Mascot";

/**
 * Full-size drop target shown above the chat while a file is dragged over the channel.
 */
const FileDropOverlay = () => {
    return (
        <div className="pointer-events-none absolute inset-0 z-30 flex items-center justify-center p-3 md:p-5">
            <div className="flex h-full w-full flex-col items-center justify-center gap-2 rounded-2xl border-2 border-dashed border-[#5D6D7B] bg-white/85 backdrop-blur-[2px]">
                <Mascot pose="running" className="mascot-bob h-24 md:h-32" />
                <span className="text-lg font-semibold text-gray-700">Drop files here</span>
                <span className="text-sm text-gray-500">They will be attached to your message</span>
            </div>
        </div>
    );
};

export default FileDropOverlay;
