namespace GleyGameServices
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    //using UnityEngine.SocialPlatforms.GameCenter;


#if UseGameCenterPlugin
using UnityEngine.SocialPlatforms.GameCenter;
#endif

    public class LeaderboardManager
    {
        //load the list of all game leaderboards form the Settings Window
        private List<Leaderboard> gameLeaderboards;


        /// <summary>
        /// Constructor, loads settings data
        /// </summary>
        public LeaderboardManager()
        {
            try
            {
                gameLeaderboards = Resources.Load<GameServicesSettings>("GameServicesData").allGameLeaderboards;
            }
            catch
            {
                Debug.LogError("Game Services Data not found -> Go to Window->Gley->Game Services to setup the plugin");
                ScreenWriter.Write("Game Services Data not found -> Go to Window->Gley->Game Services to setup the plugin");
            }
        }


        /// <summary>
        /// Submit a score for both ANdroid and iOS using the Unity Social interface
        /// </summary>
        /// <param name="score">value of the score</param>
        /// <param name="leaderboardName">leaderboard to submit score in</param>
        /// <param name="SubmitComplete">callback -> submit result</param>
        public void SubmitScore(long score, string leaderboardName, UnityAction<bool, GameServicesError> SubmitComplete)
        {
            string leaderboardId = null;
#if UseGooglePlayGamesPlugin
            leaderboardId = gameLeaderboards.FirstOrDefault(cond => cond.name == leaderboardName).idGoogle;
#endif
#if UseGameCenterPlugin
            leaderboardId = gameLeaderboards.FirstOrDefault(cond => cond.name == leaderboardName).idIos;
#endif
            Social.ReportScore(score, leaderboardId, (bool success) =>
            {
                if (success)
                {
                    if (SubmitComplete != null)
                    {
                        SubmitComplete(true, GameServicesError.Success);
                    }
                }
                else
                {
                    if (SubmitComplete != null)
                    {
                        SubmitComplete(false, GameServicesError.ScoreSubmitFailed);
                    }
                }
            });
        }


        /// <summary>
        /// Shows all game leaderboards
        /// </summary>
        public void ShowLeaderboards()
        {
            Social.ShowLeaderboardUI();
        }


        /// <summary>
        /// Displays a specific leaderboard on screen. Available only for Google Play
        /// </summary>
        /// <param name="leaderboardName">the name of the leaderboard to display</param>
        public void ShowSingleLeaderboard(LeaderboardNames leaderboardName)
        {
#if UseGooglePlayGamesPlugin
            string leaderboardId = gameLeaderboards.FirstOrDefault(cond => cond.name == leaderboardName.ToString()).idGoogle;
            ((GooglePlayGames.PlayGamesPlatform)Social.Active).ShowLeaderboardUI(leaderboardId);
            return;
#else

            string leaderboardId = gameLeaderboards.FirstOrDefault(cond => cond.name == leaderboardName.ToString()).idIos;
            GameCenterPlatform.ShowLeaderboardUI(leaderboardId, UnityEngine.SocialPlatforms.TimeScope.AllTime);
            return;
#endif

        }
    }
}
