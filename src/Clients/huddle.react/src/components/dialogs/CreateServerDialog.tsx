import { useState } from "react";
import { FiCamera, FiUploadCloud } from "react-icons/fi";
import { RxCross2 } from "react-icons/rx";
import type { Server } from "../../api/types";
import { useCreateServer } from "../../hooks/queries/servers";
import { useAvatarUpload } from "../../hooks/useAvatarUpload";
import { useFileDrop } from "../../hooks/useFileDrop";
import { resolveFileUrl } from "../../api/files/filesApi";

interface CreateServerDialogProps {
    onCreateServer: (createdServer: Server | null) => void;
}

const CreateServerDialog: React.FC<CreateServerDialogProps> = ({ onCreateServer }) => {

    const createServer = useCreateServer();
    const [serverName, setServerName] = useState('');
    const [avatarFileId, setAvatarFileId] = useState<string | null>(null);
    const [avatarUrl, setAvatarUrl] = useState<string | null>(null);

    const avatar = useAvatarUpload((file) => {
        setAvatarFileId(file.id);
        setAvatarUrl(file.url);
    });

    const { isDragging, dropHandlers } = useFileDrop({
        onFiles: (files) => void avatar.start(files[0])
    });

    const handleCreate = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (createServer.isPending || createServer.isSuccess || avatar.isUploading)
            return;

        createServer.mutate(
            { name: serverName, avatarFileId },
            {
                onSuccess: (data, variables, context) => {
                    onCreateServer(data);
                }
            }
        )
    }

    return (
        <form
            className="w-full max-w-80 space-y-4 px-2 pb-2 md:space-y-3"
            onSubmit={handleCreate}
        >
            <div className="items-left flex flex-col">
                <div className="flex flex-row items-center">
                    <h3 className="flex-grow text-2xl font-bold text-gray-700">Create new server</h3>
                    <button type="button" onClick={() => onCreateServer(null)}>
                        <RxCross2 />
                    </button>
                </div>
            </div>
            <div className="text-center">
                <span>Personalize your server by choosing a name and image. You can change them later.</span>
            </div>

            <div
                className="flex select-none flex-col items-center gap-2.5"
                {...dropHandlers}
            >
                <input {...avatar.fileInputProps} />
                <button
                    type="button"
                    onClick={avatar.open}
                    disabled={avatar.isUploading}
                    aria-label="Upload server avatar"
                    className={`group relative flex h-28 w-28 items-center justify-center overflow-hidden rounded-2xl border transition-all duration-200 focus:outline-none focus-visible:ring-2 focus-visible:ring-[#5D6D7B]/40 disabled:cursor-wait ${
                        isDragging
                            ? "border-solid border-[#5D6D7B] bg-[#5D6D7B]/10 ring-4 ring-[#5D6D7B]/20"
                            : avatarUrl
                                ? "border-transparent shadow-sm hover:shadow-md"
                                : "border-dashed border-[#5D6D7B]/40 bg-[#5D6D7B]/5 hover:border-[#5D6D7B] hover:bg-[#5D6D7B]/10"
                    }`}
                >
                    {avatarUrl ? (
                        <>
                            <img
                                src={resolveFileUrl(avatarUrl)}
                                alt="Server avatar"
                                className="h-full w-full object-cover transition-transform duration-200 group-hover:scale-105"
                            />
                            <span className={`absolute inset-0 flex flex-col items-center justify-center gap-1 bg-black/45 text-white transition-opacity duration-200 ${isDragging ? "opacity-100" : "opacity-0 group-hover:opacity-100 group-focus-visible:opacity-100"}`}>
                                {isDragging ? <FiUploadCloud className="h-5 w-5" /> : <FiCamera className="h-5 w-5" />}
                                <span className="text-[11px] font-medium">{isDragging ? "Drop" : "Change"}</span>
                            </span>
                        </>
                    ) : (
                        <span className="flex flex-col items-center gap-1.5 text-[#5D6D7B]/70 transition-colors duration-150 group-hover:text-[#5D6D7B]">
                            <FiUploadCloud className="h-8 w-8" />
                            <span className="text-[11px] font-semibold tracking-wide uppercase">Upload</span>
                        </span>
                    )}

                    {avatar.isUploading && (
                        <span className="absolute inset-0 flex flex-col items-center justify-center gap-1.5 bg-black/55 text-white backdrop-blur-[1px]">
                            <span className="text-xs font-semibold">{avatar.progress ?? 0}%</span>
                            <span className="h-1 w-3/5 overflow-hidden rounded-full bg-white/30">
                                <span
                                    className="block h-full rounded-full bg-white transition-[width] duration-150"
                                    style={{ width: `${avatar.progress ?? 0}%` }}
                                />
                            </span>
                        </span>
                    )}
                </button>

                <span className={`text-center text-[11px] leading-tight ${isDragging ? "font-medium text-[#5D6D7B]" : "text-gray-500"}`}>
                    {isDragging
                        ? "Drop to upload"
                        : avatarUrl
                            ? "Click the image to change it"
                            : "PNG, JPEG, GIF or WebP · up to 5 MB"}
                </span>
            </div>

            {avatar.error && (
                <div className="text-center text-xs font-medium text-red-500">{avatar.error}</div>
            )}

            <div>
                <label className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">Server name</label>
                <input
                    onChange={(event: React.ChangeEvent<HTMLInputElement>) => setServerName(event.target.value)}
                    autoFocus={true}
                    type="text"
                    name="project-name"
                    className="p-2.5 block w-full rounded-lg border border-gray-300 bg-gray-50 text-sm text-gray-900 focus:ring-primary-600 focus:border-primary-600 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-[#5D6D7B] dark:focus:border-[#5D6D7B]"
                    required
                    placeholder="new-server"
                />
            </div>
            <div className="flex w-full flex-row justify-end">
                <button
                    type="submit"
                    className="mt-1 w-25 rounded-md bg-[#5D6D7B] p-2 font-medium text-white transition-colors duration-150 hover:bg-[#4F5E6A] disabled:opacity-50"
                    disabled={createServer.isPending || avatar.isUploading}
                >
                    {createServer.isPending ? 'Creating...' : 'Create'}
                </button>
            </div>

        </form>
    );

}

export default CreateServerDialog;
