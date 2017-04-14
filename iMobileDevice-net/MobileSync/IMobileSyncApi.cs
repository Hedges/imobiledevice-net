// <copyright file="IMobileSyncApi.cs" company="Quamotion">
// Copyright (c) 2016-2017 Quamotion. All rights reserved.
// </copyright>

namespace iMobileDevice.MobileSync
{
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using iMobileDevice.iDevice;
    using iMobileDevice.Lockdown;
    using iMobileDevice.Afc;
    using iMobileDevice.Plist;
    
    
    public partial interface IMobileSyncApi
    {
        
        /// <summary>
        /// Gets or sets the <see cref="ILibiMobileDeviceApi"/> which owns this <see cref="MobileSync"/>.
        /// </summary>
        ILibiMobileDevice Parent
        {
            get;
        }
        
        /// <summary>
        /// Connects to the mobilesync service on the specified device.
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one or more parameters are invalid
        /// DEVICE_LINK_SERVICE_E_BAD_VERSION if the mobilesync version on
        /// the device is newer.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="service">
        /// The service descriptor returned by lockdownd_start_service.
        /// </param>
        /// <param name="client">
        /// Pointer that will be set to a newly allocated
        /// #mobilesync_client_t upon successful return.
        /// </param>
        MobileSyncError mobilesync_client_new(iDeviceHandle device, LockdownServiceDescriptorHandle service, out MobileSyncClientHandle client);
        
        /// <summary>
        /// Starts a new mobilesync service on the specified device and connects to it.
        /// </summary>
        /// <param name="device">
        /// The device to connect to.
        /// </param>
        /// <param name="client">
        /// Pointer that will point to a newly allocated
        /// mobilesync_client_t upon successful return. Must be freed using
        /// mobilesync_client_free() after use.
        /// </param>
        /// <param name="label">
        /// The label to use for communication. Usually the program name.
        /// Pass NULL to disable sending the label in requests to lockdownd.
        /// </param>
        /// <returns>
        /// MOBILESYNC_E_SUCCESS on success, or an MOBILESYNC_E_* error
        /// code otherwise.
        /// </returns>
        MobileSyncError mobilesync_client_start_service(iDeviceHandle device, out MobileSyncClientHandle client, string label);
        
        /// <summary>
        /// Disconnects a mobilesync client from the device and frees up the
        /// mobilesync client data.
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if
        /// is NULL.
        /// </summary>
        /// <param name="client">
        /// The mobilesync client to disconnect and free.
        /// </param>
        MobileSyncError mobilesync_client_free(System.IntPtr client);
        
        /// <summary>
        /// Polls the device for mobilesync data.
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        /// <param name="plist">
        /// A pointer to the location where the plist should be stored
        /// </param>
        /// <returns>
        /// an error code
        /// </returns>
        MobileSyncError mobilesync_receive(MobileSyncClientHandle client, out PlistHandle plist);
        
        /// <summary>
        /// Sends mobilesync data to the device
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        /// <param name="plist">
        /// The location of the plist to send
        /// </param>
        /// <returns>
        /// an error code
        /// </returns>
        /// <remarks>
        /// This function is low-level and should only be used if you need to send
        /// a new type of message.
        /// </remarks>
        MobileSyncError mobilesync_send(MobileSyncClientHandle client, PlistHandle plist);
        
        /// <summary>
        /// Requests starting synchronization of a data class with the device
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid
        /// MOBILESYNC_E_PLIST_ERROR if the received plist is not of valid form
        /// MOBILESYNC_E_SYNC_REFUSED if the device refused to sync
        /// MOBILESYNC_E_CANCELLED if the device explicitly cancelled the
        /// sync request
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        /// <param name="data_class">
        /// The data class identifier to sync
        /// </param>
        /// <param name="anchors">
        /// The anchors required to exchange with the device. The anchors
        /// allow the device to tell if the synchronization information on the computer
        /// and device are consistent to determine what sync type is required.
        /// </param>
        /// <param name="computer_data_class_version">
        /// The version of the data class storage on the computer
        /// </param>
        /// <param name="sync_type">
        /// A pointer to store the sync type reported by the device_anchor
        /// </param>
        /// <param name="device_data_class_version">
        /// The version of the data class storage on the device
        /// </param>
        /// <param name="error_description">
        /// A pointer to store an error message if reported by the device
        /// </param>
        MobileSyncError mobilesync_start(MobileSyncClientHandle client, byte[] dataClass, MobileSyncAnchorsHandle anchors, ulong computerDataClassVersion, ref MobileSyncSyncType syncType, ref ulong deviceDataClassVersion, out string errorDescription);
        
        /// <summary>
        /// Cancels a running synchronization session with a device at any time.
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        /// <param name="reason">
        /// The reason to supply to the device for cancelling
        /// </param>
        MobileSyncError mobilesync_cancel(MobileSyncClientHandle client, string reason);
        
        /// <summary>
        /// Finish a synchronization session of a data class on the device.
        /// A session must have previously been started using mobilesync_start().
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid
        /// MOBILESYNC_E_PLIST_ERROR if the received plist is not of valid
        /// form
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        MobileSyncError mobilesync_finish(MobileSyncClientHandle client);
        
        /// <summary>
        /// Requests to receive all records of the currently set data class from the device.
        /// The actually changes are retrieved using mobilesync_receive_changes() after this
        /// request has been successful.
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        MobileSyncError mobilesync_get_all_records_from_device(MobileSyncClientHandle client);
        
        /// <summary>
        /// Requests to receive only changed records of the currently set data class from the device.
        /// The actually changes are retrieved using mobilesync_receive_changes() after this
        /// request has been successful.
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        MobileSyncError mobilesync_get_changes_from_device(MobileSyncClientHandle client);
        
        /// <summary>
        /// Requests the device to delete all records of the current data class
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid
        /// MOBILESYNC_E_PLIST_ERROR if the received plist is not of valid form
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        /// <remarks>
        /// The operation must be called after starting synchronization.
        /// </remarks>
        MobileSyncError mobilesync_clear_all_records_on_device(MobileSyncClientHandle client);
        
        /// <summary>
        /// Receives changed entitites of the currently set data class from the device
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid
        /// MOBILESYNC_E_CANCELLED if the device explicitly cancelled the
        /// session
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        /// <param name="entities">
        /// A pointer to store the changed entity records as a PLIST_DICT
        /// </param>
        /// <param name="is_last_record">
        /// A pointer to store a flag indicating if this submission is the last one
        /// </param>
        /// <param name="actions">
        /// A pointer to additional flags the device is sending or NULL to ignore
        /// </param>
        MobileSyncError mobilesync_receive_changes(MobileSyncClientHandle client, out PlistHandle entities, ref char isLastRecord, out PlistHandle actions);
        
        /// <summary>
        /// Acknowledges to the device that the changes have been merged on the computer
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        MobileSyncError mobilesync_acknowledge_changes_from_device(MobileSyncClientHandle client);
        
        /// <summary>
        /// Verifies if the device is ready to receive changes from the computer.
        /// This call changes the synchronization direction so that mobilesync_send_changes()
        /// can be used to send changes to the device.
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid
        /// MOBILESYNC_E_PLIST_ERROR if the received plist is not of valid form
        /// MOBILESYNC_E_WRONG_DIRECTION if the current sync direction does
        /// not permit this call
        /// MOBILESYNC_E_CANCELLED if the device explicitly cancelled the
        /// session
        /// MOBILESYNC_E_NOT_READY if the device is not ready to start
        /// receiving any changes
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        MobileSyncError mobilesync_ready_to_send_changes_from_computer(MobileSyncClientHandle client);
        
        /// <summary>
        /// Sends changed entities of the currently set data class to the device
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid,
        /// MOBILESYNC_E_WRONG_DIRECTION if the current sync direction does
        /// not permit this call
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        /// <param name="entities">
        /// The changed entity records as a PLIST_DICT
        /// </param>
        /// <param name="is_last_record">
        /// A flag indicating if this submission is the last one
        /// </param>
        /// <param name="actions">
        /// Additional actions for the device created with mobilesync_actions_new()
        /// or NULL if no actions should be passed
        /// </param>
        MobileSyncError mobilesync_send_changes(MobileSyncClientHandle client, PlistHandle entities, char isLastRecord, PlistHandle actions);
        
        /// <summary>
        /// Receives any remapped identifiers reported after the device merged submitted changes.
        /// MOBILESYNC_E_SUCCESS on success
        /// MOBILESYNC_E_INVALID_ARG if one of the parameters is invalid
        /// MOBILESYNC_E_PLIST_ERROR if the received plist is not of valid
        /// form
        /// MOBILESYNC_E_WRONG_DIRECTION if the current sync direction does
        /// not permit this call
        /// MOBILESYNC_E_CANCELLED if the device explicitly cancelled the
        /// session
        /// </summary>
        /// <param name="client">
        /// The mobilesync client
        /// </param>
        /// <param name="mapping">
        /// A pointer to an array plist containing a dict of identifier remappings
        /// </param>
        MobileSyncError mobilesync_remap_identifiers(MobileSyncClientHandle client, out PlistHandle mapping);
        
        /// <summary>
        /// Allocates memory for a new anchors struct initialized with the passed anchors.
        /// MOBILESYNC_E_SUCCESS on success
        /// </summary>
        /// <param name="device_anchor">
        /// An anchor the device reported the last time or NULL
        /// if none is known yet which for instance is true on first synchronization.
        /// </param>
        /// <param name="computer_anchor">
        /// An arbitrary string to use as anchor for the computer.
        /// </param>
        /// <param name="client">
        /// Pointer that will be set to a newly allocated
        /// #mobilesync_anchors_t struct. Must be freed using mobilesync_anchors_free().
        /// </param>
        MobileSyncError mobilesync_anchors_new(string deviceAnchor, string computerAnchor, out MobileSyncAnchorsHandle anchor);
        
        /// <summary>
        /// Free memory used by anchors.
        /// MOBILESYNC_E_SUCCESS on success
        /// </summary>
        /// <param name="anchors">
        /// The anchors to free.
        /// </param>
        MobileSyncError mobilesync_anchors_free(System.IntPtr anchors);
        
        /// <summary>
        /// Create a new actions plist to use in mobilesync_send_changes().
        /// </summary>
        /// <returns>
        /// A new plist_t of type PLIST_DICT.
        /// </returns>
        PlistHandle mobilesync_actions_new();
        
        /// <summary>
        /// Add one or more new key:value pairs to the given actions plist.
        /// </summary>
        /// <param name="actions">
        /// The actions to modify.
        /// </param>
        /// <param name="...">
        /// KEY, VALUE, [KEY, VALUE], NULL
        /// </param>
        /// <remarks>
        /// The known keys so far are "SyncDeviceLinkEntityNamesKey" which expects
        /// an array of entity names, followed by a count paramter as well as
        /// "SyncDeviceLinkAllRecordsOfPulledEntityTypeSentKey" which expects an
        /// integer to use as a boolean value indicating that the device should
        /// link submitted changes and report remapped identifiers.
        /// </remarks>
        void mobilesync_actions_add(PlistHandle actions);
        
        /// <summary>
        /// Free actions plist.
        /// </summary>
        /// <param name="actions">
        /// The actions plist to free. Does nothing if NULL is passed.
        /// </param>
        void mobilesync_actions_free(PlistHandle actions);
    }
}