using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace hx2src
{
  internal class Program
  {
    private static void Main(string[] args)
    {

      var client = new RestClient("http://localhost:9091/");

      var requCgm = new RestRequest("definitions/schema/cgm/*");
      IRestResponse<List<SchemaDef>> respCgm = client.Execute<List<SchemaDef>>(requCgm);

      var requStorm = new RestRequest("definitions/schema/com/cgm/storm/*");
      IRestResponse<List<SchemaDef>> respStorm = client.Execute<List<SchemaDef>>(requStorm);


      var requCgmComponents = new RestRequest("definitions/component/cgm/*");
      IRestResponse<List<ComponentDef>> respCgmComponents = client.Execute<List<ComponentDef>>(requCgmComponents);

      var requStormComponents = new RestRequest("definitions/component/com/cgm/storm/*");
      IRestResponse<List<ComponentDef>> respStormComponents = client.Execute<List<ComponentDef>>(requStormComponents);

      foreach (SchemaDef def in respStorm.Data)
      {
        Console.Write(Program.CreateSchemaInterfaces(def));
      }


    }


    private static String CreateSchemaInterfaces(SchemaDef def)
    {
      StringBuilder sb = new StringBuilder();

      foreach (String key in def.types.Keys)
      {
        sb.Append(Program.CreateEntityInterface(key, def.types[key]));
      }
      return sb.ToString();
    }

    private static String CreateEntityInterface(String name, EntitySchema entity)
    {
      StringBuilder sb = new StringBuilder();

      sb.Append("interface I");
      sb.Append(name);
      sb.Append(" {");
      sb.AppendLine();

      foreach (var elemName in entity.elements.Keys)
      {
        var element = entity.elements[elemName];
        
        sb.Append("    ");
        sb.Append(elemName);

        if (!element.mandatory) {
          sb.Append("?");
        }


        sb.Append(": ");

        sb.Append(CreateEntityElementType(element));
        sb.Append(";");
        sb.AppendLine();
      }

      sb.Append("}");
      sb.AppendLine();
      return sb.ToString();
    }

    private static String CreateEntityElementType(EntityElement element)
    {
      switch(element.code) {
        case "SIMPLE_TYPE":
          return element.primitive;
        case "COMPLEX_TYPE":
          return element._base;
      }
      return "any";

    }
  }

  class SchemaDef
  {

    public SchemaDef() {
    }

    public string name { get; set; }
    public string __definitionType { get; set; }
    public Dictionary<string,EntitySchema> types { get; set; }
  }

  class EntitySchema
  {
    public EntitySchema(){
    }

    public string name { get; set; }
    public string _base { get; set; }
    public string code { get; set; }

    public string isArray { get; set; }
    public Object annotations { get; set; }
    public Boolean companyAware { get; set; }
    public Boolean customized { get; set; }
    public Boolean deleteLogicalEnabled { get; set; }
    public Boolean orgUnitAware { get; set; }
    public Boolean stampChanges { get; set; }
    public Boolean timeDependentEnabled { get; set; }

    public Dictionary<string, EntityElement> elements { get; set; }
    
  }

  public class EntityElement
  {
    public EntityElement()
    {     
    }

    public string name { get; set; }
    public string _base { get; set; }
    public string code  { get; set; }
    public Boolean isArray  { get; set; }
    public Object annotations  { get; set; }
    public string primitive  { get; set; }
    public Object defaultValue  { get; set; }
    public Boolean mandatory { get; set; }
    public int decimals  { get; set; }
    public int length { get; set; }
    public Boolean isResolved  { get; set; }
  }


  public class ComponentDef
  {
      public ComponentDef()
      {
      }

      public string name { get; set; }
      public string __definitionType { get; set; }
      public Dictionary<string, ProcedureDef> procedures { get; set; }

  }

  public class ProcedureDef
  {
    public ProcedureDef()
    {
    
    }

    public List<ParamDef> input { get; set; }
    public List<ParamDef> output { get; set; }
  }

  public class ParamDef
  {
    public ParamDef()
    {
    }
    public string name { get; set; }
    public string type { get; set; }
    public Boolean isArray { get; set; }
  }
}
