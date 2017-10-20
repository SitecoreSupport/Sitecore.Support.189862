using System;
using Sitecore.FXM.Utilities;

namespace Sitecore.Support.FXM.Client.Pipelines.ExperienceEditor.ExternalPage
{
  using System.Web.UI;

  using HtmlAgilityPack;

  using Sitecore.Abstractions;
  using Sitecore.Diagnostics;
  using Sitecore.FXM.Client.Pipelines.ExperienceEditor.ExternalPage;

  /// <summary>Placeholder control injection pipeline processor.</summary>
  public class InjectControlsProcessor : ExternalPageProcessorBase, IExternalPageExperienceEditorProcessor
  {
    public InjectControlsProcessor()
      : this(new SettingsWrapper())
    {
    }

    public InjectControlsProcessor(ISettings settings)
      : base(settings)
    {
    }

    /// <summary>Generates the Placeholder controls for the external site page.</summary>
    /// <param name="args">The args.</param>
    public void Process(ExternalPageExperienceEditorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.MatcherContextItem, "MatcherContextItem");
      Assert.ArgumentNotNullOrEmpty(args.ExternalPageContent, "ExternalPageContent");

      HtmlDocument htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(args.ExternalPageContent);

      LiteralControl headControl = new LiteralControl();
      var headSingleNode = htmlDoc.DocumentNode.SelectSingleNode("//head");
      if (headSingleNode != null)
      {
        RemoveBeaconFromNode(headSingleNode);
        headControl.Text = headSingleNode.InnerHtml;
      }

      args.HeadControls.Add(headControl);

      string bodyHtml = string.Empty;
      var bodySingleNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
      if (bodySingleNode != null)
      {
        Uri experienceEditorUrl;
        if (Uri.TryCreate(args.ExperienceEditorUrl, UriKind.RelativeOrAbsolute, out experienceEditorUrl))
        {
          this.HostName = FxmUtility.GetUriHost(experienceEditorUrl);
        }

        RemoveBeaconFromNode(bodySingleNode);

        bodyHtml = bodySingleNode.InnerHtml;
      }

      args.BodyControls.Add(new LiteralControl(bodyHtml));
    }

    protected virtual void RemoveBeaconFromNode(HtmlNode node)
    {
      var beaconTag = this.GetBeaconScriptTag(node);
      if (beaconTag != null)
      {
        this.RemoveNode(beaconTag);
      }
    }
  }
}