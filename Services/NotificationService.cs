using AutoMapper;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Firebase.Database.Extensions;
using VoiceAPI.Constants;
using VoiceAPI.IServices;
using VoiceAPI.Models.Common;
using VoiceAPI.Models.Data.Notifications;
using VoiceAPI.Models.Payload.Notifications;
using VoiceAPI.Models.Responses.Notifications;

namespace VoiceAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly string FIREBASE_DATABASE_URL = "https://testchat-fcbaa-default-rtdb.firebaseio.com";

        private readonly FirebaseClient _firebaseClient;

        private readonly IMapper _mapper;

        public NotificationService(IMapper mapper)
        {
            _mapper = mapper;

            _firebaseClient = new FirebaseClient(FIREBASE_DATABASE_URL);
        }

        public async Task<GenericResult<NotifyBaseDTO>> PostNotification(NotificationPayload payload)
        {
            var dataModel = _mapper.Map<NotifyBaseDataModel>(payload);
            dataModel.IsRead = false;
            dataModel.CreatedTime = DateTime.UtcNow;

            await _firebaseClient
                .Child("Notification")
                .PostAsync(dataModel);

            var response = _mapper.Map<NotifyBaseDTO>(dataModel);

            return GenericResult<NotifyBaseDTO>.Success(response);
        }

        private async Task ExecuteNotify(NotifyBaseDataModel dataModel, string child)
        {
            dataModel.IsRead = false;
            dataModel.CreatedTime = DateTime.Now;

            await _firebaseClient
                .Child(child)
                .PostAsync(dataModel);
        }

        private async Task<List<NotifyCandidateInvitedFetchDTO>> RetrieveCandidateInvitedData()
        {
            var readonlyNotifies = await _firebaseClient.Child(NotificationConstant.CHILD_CANDIDATE_INVITED)
                .OnceAsync<NotifyCandidateInvitedDataModel>();

            var firebaseObjects = new List<FirebaseObject<NotifyCandidateInvitedDataModel>>(readonlyNotifies);

            List<NotifyCandidateInvitedFetchDTO> notifies = new();
            foreach (var tempFirebaseObj in firebaseObjects)
            {
                notifies.Add(new NotifyCandidateInvitedFetchDTO
                {
                    Id = tempFirebaseObj.Key, Data = tempFirebaseObj.Object
                });
            }

            return notifies;
        }

        public async Task<GenericResult<NotifyCandidateInvitedDTO>> PostNotifyCandidateInvited(
            NotifyCandidateInvitedDataModel dataModel)
        {
            await ExecuteNotify(dataModel, NotificationConstant.CHILD_CANDIDATE_INVITED);

            var response = _mapper.Map<NotifyCandidateInvitedDTO>(dataModel);

            return GenericResult<NotifyCandidateInvitedDTO>.Success(response);
        }

        public async Task<GenericResult<NotifyJobHaveNewApplicantDTO>> PostNotifyJobHaveNewApplicant(
            NotifyJobHaveNewApplicantDataModel dataModel)
        {
            await ExecuteNotify(dataModel, NotificationConstant.CHILD_JOB_HAVE_NEW_APPLICANT);

            var response = _mapper.Map<NotifyJobHaveNewApplicantDTO>(dataModel);

            return GenericResult<NotifyJobHaveNewApplicantDTO>.Success(response);
        }

        public async Task<GenericResult<NotifyConversationScheduleDTO>> PostNotifyConversationScheduled(
            NotifyConversationScheduleDataModel dataModel)
        {
            await ExecuteNotify(dataModel, NotificationConstant.CHILD_CONVERSATION_SCHEDULE);

            var response = _mapper.Map<NotifyConversationScheduleDTO>(dataModel);

            return GenericResult<NotifyConversationScheduleDTO>.Success(response);
        }

        public async Task<GenericResult<bool>> SeenNotify(NotifyCandidateInvitedDataModel dataModel)
        {
            var notifies = await RetrieveCandidateInvitedData();

            foreach (var tempNotify in notifies)
            {
                if (tempNotify.Data.IsRead == false
                    && tempNotify.Data.TargetAccountId == dataModel.TargetAccountId
                    && tempNotify.Data.JobId == dataModel.JobId
                    && tempNotify.Data.EnterpriseId == dataModel.EnterpriseId)
                {
                    tempNotify.Data.IsRead = true;

                    await _firebaseClient
                        .Child(NotificationConstant.CHILD_CANDIDATE_INVITED + $"/{tempNotify.Id}")
                        .PutAsync(tempNotify.Data);

                    return GenericResult<bool>.Success(true);
                }
            }
            
            return GenericResult<bool>.Success(false);
        }
    }
}