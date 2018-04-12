using System;
using Sitecore.FXM.Utilities;
using System.Web.UI;
using HtmlAgilityPack;
using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.FXM.Client.Pipelines.ExperienceEditor.ExternalPage;

namespace Sitecore.Support.FXM.Client.Pipelines.ExperienceEditor.ExternalPage
{
  public class InjectControlsProcessor : ExternalPageProcessorBase, IExternalPageExperienceEditorProcessor
  {
    public InjectControlsProcessor(BaseSettings settings)
      : base(settings)
    {
    }

    public void Process(ExternalPageExperienceEditorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.MatcherContextItem, "MatcherContextItem");
      Assert.ArgumentNotNullOrEmpty(args.ExternalPageContent, "ExternalPageContent");

      var htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(args.ExternalPageContent);

      var headControl = new LiteralControl();
      var headSingleNode = htmlDoc.DocumentNode.SelectSingleNode("//head");
      if (headSingleNode != null)
      {
        #region Added code
        RemoveBeaconFromNode(headSingleNode);
        #endregion
        headControl.Text = headSingleNode.InnerHtml;
      }

      args.HeadControls.Add(headControl);

      var bodyHtml = string.Empty;
      var bodySingleNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
      if (bodySingleNode != null)
      {
        Uri experienceEditorUrl;
        if (Uri.TryCreate(args.ExperienceEditorUrl, UriKind.RelativeOrAbsolute, out experienceEditorUrl))
        {
          HostName = FxmUtility.GetUriHost(experienceEditorUrl);
        }

        #region Modified code
        RemoveBeaconFromNode(bodySingleNode);
        #endregion

        bodyHtml = bodySingleNode.InnerHtml;
      }

      args.BodyControls.Add(new LiteralControl(bodyHtml));
    }

    #region Added code
    protected virtual void RemoveBeaconFromNode(HtmlNode node)
    {
      var beaconTag = this.GetBeaconScriptTag(node);
      if (beaconTag != null)
      {
        this.RemoveNode(beaconTag);
      }
    }
    #endregion
  }
}