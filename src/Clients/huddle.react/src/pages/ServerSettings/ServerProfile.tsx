import { useEffect, useState } from "react";
import { useParams } from "react-router";
import { useServer, useUpdateServer } from "../../hooks/queries/servers";
import type { UpdateServerRequest } from "../../api/types";
import { useAvatarUpload } from "../../hooks/useAvatarUpload";
import { useFileDrop } from "../../hooks/useFileDrop";
import ServerAvatar from "../../components/server/ServerAvatar";
import { SettingsSkeleton } from "../../components/skeleton/Skeletons";

interface PendingAvatar {
  fileId: string;
  url: string;
}

function ServerProfile() {
  const { serverId } = useParams();

  const { data: server, isPending, error } = useServer(serverId || "");
  const updateServer = useUpdateServer();

  const [draftName, setDraftName] = useState<string>("");
  const [draftDescription, setdraftDescription] = useState<string>("");
  const [pendingAvatar, setPendingAvatar] = useState<PendingAvatar | null>(null);
  const [isAvatarRemoved, setIsAvatarRemoved] = useState(false);

  const avatar = useAvatarUpload((file) => {
    setPendingAvatar({ fileId: file.id, url: file.url });
    setIsAvatarRemoved(false);
  });

  const { isDragging, dropHandlers } = useFileDrop({
    onFiles: (files) => void avatar.start(files[0])
  });

  useEffect(() => {
    setDraftName(server?.name ?? "");
  }, [server?.name]);

  const currentAvatarUrl = pendingAvatar?.url ?? (isAvatarRemoved ? null : server?.avatarUrl ?? null);
  const hasChanges =
    draftName.trim() !== (server?.name ?? "") || pendingAvatar !== null || isAvatarRemoved;

  const [isToastOpen, setIsToastOpen] = useState(false);
  const [toastError, setToastError] = useState<string | null>(null);

  useEffect(() => {
    if (!serverId) return;

    if (hasChanges) {
      setToastError(null);
      setIsToastOpen(true);
    } else {
      setToastError(null);
      setIsToastOpen(false);
    }
  }, [hasChanges, serverId]);

  if (!serverId) {
    return (
      <div className="flex h-full w-full items-center justify-center">
        <div className="text-center text-gray-700">Invalid server ID</div>
      </div>
    );
  }

  if (isPending) {
    return <SettingsSkeleton />;
  }

  if (error) {
    return (
      <div className="flex h-full w-full items-center justify-center">
        <div className="text-center text-red-600">Error getting server data</div>
      </div>
    );
  }

  const handleCancel = () => {
    setToastError(null);
    setDraftName(server?.name ?? "");
    setPendingAvatar(null);
    setIsAvatarRemoved(false);
    setIsToastOpen(false);
  };

  const handleSave = () => {
    if (!serverId) return;
    setToastError(null);

    const data: UpdateServerRequest = {
      name: draftName.trim(),
      avatarFileId: pendingAvatar?.fileId ?? null,
      removeAvatar: isAvatarRemoved,
    };

    updateServer.mutate(
      { id: serverId, data },
      {
        onSuccess: () => {
          setPendingAvatar(null);
          setIsAvatarRemoved(false);
          setIsToastOpen(false);
        },
        onError: () => {
          setToastError("Error");
        },
      }
    );
  };

  const handleRemoveAvatar = () => {
    setPendingAvatar(null);
    setIsAvatarRemoved(true);
  };

  const serverInitial = (draftName.trim() || server?.name || "S").charAt(0).toUpperCase();

  return (
    <div className="relative flex h-full w-full gap-6 rounded-tl-2xl bg-gray-100 p-6">
      <div className="flex w-full min-w-0 flex-col items-start gap-4">
        <h2 className="text-lg font-semibold text-gray-700">Server profile</h2>
        <p className="text-sm text-gray-500">
          Update server information
        </p>

        {/* Avatar */}

        <div
          className="flex items-center gap-4"
          {...dropHandlers}
        >
          <input {...avatar.fileInputProps} />
          <button
            type="button"
            onClick={avatar.open}
            aria-label="Change server avatar"
            className={`relative flex h-20 w-20 shrink-0 items-center justify-center overflow-hidden rounded-2xl border border-dashed text-xl font-semibold text-white transition-colors duration-150 ${isDragging ? "border-[#5D6D7B] bg-gray-300" : "border-gray-300 bg-[#5D6D7B]"}`}
          >
            {currentAvatarUrl ? (
              <ServerAvatar
                name={server?.name ?? ""}
                avatarUrl={currentAvatarUrl}
                className="flex h-full w-full items-center justify-center"
                imageClassName="h-full w-full object-cover"
              />
            ) : (
              serverInitial
            )}

            {avatar.isUploading && (
              <div className="absolute inset-0 flex flex-col items-center justify-center gap-1 bg-black/50">
                <span className="text-xs font-semibold">{avatar.progress ?? 0}%</span>
                <div className="h-1 w-3/4 overflow-hidden rounded-full bg-white/30">
                  <div
                    className="h-full rounded-full bg-white transition-[width] duration-150"
                    style={{ width: `${avatar.progress ?? 0}%` }}
                  />
                </div>
              </div>
            )}
          </button>

          <div className="flex flex-col gap-2">
            <div className="flex flex-wrap gap-2">
              <button
                type="button"
                onClick={avatar.open}
                disabled={avatar.isUploading}
                className="rounded-md bg-gray-500 px-3 py-1 font-medium text-gray-100 transition-colors hover:bg-gray-600 disabled:opacity-50"
              >
                {avatar.isUploading ? "Uploading..." : "Change server avatar"}
              </button>
              <button
                type="button"
                onClick={handleRemoveAvatar}
                disabled={!currentAvatarUrl}
                className="rounded-md border bg-gray-200 px-3 py-1 font-medium text-red-400 transition-colors hover:bg-gray-300 disabled:opacity-50"
              >
                Remove avatar
              </button>
            </div>
            <span className="text-xs text-gray-500">
              PNG, JPEG, GIF or WebP up to 5 MB. Drag a file onto the icon to upload.
              {hasChanges && " Changes are applied when you save."}
            </span>
            {avatar.error && <span className="text-xs font-medium text-red-500">{avatar.error}</span>}
            {updateServer.isError && (
              <span className="text-xs font-medium text-red-500">Could not update the server</span>
            )}
          </div>
        </div>

        {/* Server Name */}

        <label className="text-sm font-medium text-gray-700">Server name</label>
        <input
          value={draftName}
          onChange={(e) => setDraftName(e.target.value)}
          type="text"
          className="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-gray-400 focus:border-gray-400"
          placeholder="server name"
        />

        {/* Description */}
        <label className="text-sm font-medium text-gray-700">Description</label>
        <span className="text-xs font-medium text-gray-500">Why did you create the server? Why should users join it?</span>
        <textarea
          value={draftDescription}
          rows={5}
          onChange={(e) => setdraftDescription(e.target.value)}
          className="block resize-none w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:outline-none focus:ring-2 focus:ring-gray-400 focus:border-gray-400"
          placeholder="Tell the world about this server"
        />

      </div>

      {/* Server card preview. The background will become configurable later, for now it stays static. */}
      <aside className="hidden w-80 shrink-0 lg:block">
        <div className="flex flex-col items-center gap-4 rounded-xl bg-gray-200 p-6 shadow-inner">
          <div className="flex h-20 w-20 items-center justify-center overflow-hidden rounded-2xl bg-[#5D6D7B] text-2xl font-semibold text-white">
            {currentAvatarUrl ? (
              <ServerAvatar
                name={server?.name ?? ""}
                avatarUrl={currentAvatarUrl}
                className="flex h-full w-full items-center justify-center"
                imageClassName="h-full w-full object-cover"
              />
            ) : (
              serverInitial
            )}
          </div>
          <div className="flex w-full flex-col items-center gap-1 text-center">
            <span className="w-full truncate text-base font-semibold text-gray-700">
              {draftName.trim() || server?.name || "Server"}
            </span>
            <span className="text-xs text-gray-500">Server card preview</span>
          </div>
        </div>
      </aside>

      {isToastOpen && (
        <div className="absolute bottom-4 z-50">
          <div className="rounded-xl border border-gray-300 bg-white p-4 shadow-2xl">
            <div className="flex flex-row items-center justify-center gap-3 align-middle">
              <div className="flex h-10 w-10 items-center justify-center rounded-md bg-gray-500 text-white">
                ✎
              </div>
              <div className="flex flex-1 flex-col">
                <span className="text-sm font-semibold text-gray-700">
                  There are unsaved changes
                </span>
                {toastError ? (
                  <span className="mt-1 text-sm text-red-600">
                    {toastError}
                  </span>
                ) : (
                  <span className="mt-1 text-sm text-gray-500">
                    Save or cancel your changes
                  </span>
                )}
              </div>
              <div className="flex flex-row justify-end gap-3">
                <button
                  type="button"
                  onClick={handleCancel}
                  className="rounded-md px-4 py-2 font-medium text-gray-600 transition-colors hover:text-gray-700"
                  disabled={updateServer.isPending}
                >
                  Cancel
                </button>
                <button
                  type="button"
                  onClick={handleSave}
                  className="rounded-md bg-gray-500 px-4 py-2 font-medium text-white transition-colors hover:bg-gray-600 disabled:opacity-50"
                  disabled={!hasChanges || updateServer.isPending || avatar.isUploading}
                >
                  {updateServer.isPending ? "Saving..." : "Save"}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default ServerProfile;
