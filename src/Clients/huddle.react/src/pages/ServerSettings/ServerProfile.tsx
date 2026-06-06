import { useEffect, useMemo, useState } from "react";
import { useParams } from "react-router";
import { useServer, useUpdateServer } from "../../hooks/queries/servers";
import type { UpdateServerRequest } from "../../api/types";

function ServerProfile() {
  const { serverId } = useParams();

  const { data: server, isPending, error } = useServer(serverId || "");
  const updateServer = useUpdateServer();

  const [draftName, setDraftName] = useState<string>("");
  const [draftDescription, setdraftDescription] = useState<string>("");

  useEffect(() => {
    setDraftName(server?.name ?? "");
  }, [server?.name]);

  const hasChanges = draftName.trim() !== (server?.name ?? "");

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
    return (
      <div className="flex h-full w-full items-center justify-center">
        <div className="h-8 w-8 animate-spin rounded-full border-b-2 border-gray-900" />
      </div>
    );
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
    setIsToastOpen(false);
  };

  const handleSave = () => {
    if (!serverId) return;
    setToastError(null);

    const data: UpdateServerRequest = {
      name: draftName.trim(),
    };

    updateServer.mutate(
      { id: serverId, data },
      {
        onSuccess: () => {
          setIsToastOpen(false);
        },
        onError: () => {
          setToastError("Error");
        },
      }
    );
  };

  return (
    <div className="relative flex h-full w-full gap-6 rounded-tl-2xl bg-gray-100 p-6">
      <div className="flex w-full flex-col items-start gap-4">
        <h2 className="text-lg font-semibold text-gray-700">Server profile</h2>
        <p className="text-sm text-gray-500">
          Update server information
        </p>

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

        {/* Avatar */}
        <label className="text-sm font-medium text-gray-700">Avatar</label>
        <div className="flex gap-2">
          <button type="button" className="rounded-md bg-gray-500 px-3 py-1 font-medium text-gray-100">Change server avatar</button>
          <button type="button" className="rounded-md border bg-gray-200 px-3 py-1 font-medium text-red-400">Remove avatar</button>
        </div>


      </div>

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
                  disabled={!hasChanges || updateServer.isPending}
                >
                  {updateServer.isPending ? "Saving..." : "Save"}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      <div className="h-60 w-90 rounded-xl bg-gray-200">

      </div>
    </div>
  );
}

export default ServerProfile;

