/*******************************************************************************
* Author    :  Angus Johnson                                                   *
* Date      :  16 December 2025                                                *
* Website   :  https://www.angusj.com                                          *
* Copyright :  Angus Johnson 2010-2025                                         *
* License   :  https://www.boost.org/LICENSE_1_0.txt                           *
*******************************************************************************/

using System.Formats.Asn1;
using System.Globalization;
using System;
using System.IO;
using System.Reflection;
using CsvHelper;
 


#if USINGZ
using Clipper2ZLib;
#else
using Clipper2Lib;
#endif

namespace InflateDemo
{
  public class Record
  {
    public double x { get; set; }
    public double y { get; set; }
  }
  public static class Application
  {

    public static void Main(string[] args)
    {
      ParseResult parseResult = rootCommand.Parse(args);
      PathD data = new PathD();
      PathsD dataSets = new PathsD();
      using (var reader = new StreamReader("d:/del/test.csv"))
      using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
      {
        var records = csv.GetRecords<Record>();

      
        foreach (var record in records)
        {
          PointD rec = new Clipper2Lib.PointD(record.x, record.y);
          data.Add(rec);
          //athsD rec = new() { Clipper.MakePath(new double[] { record.x, record.y }) };
          Console.WriteLine($"{record.x} and {record.y}");
        }
      }
      dataSets.Add(data);

      PathsD ppp1 = new() { Clipper.MakePath(new double[] { 30, 150,40,160 }) };
      PathsD ppp2 = new() { Clipper.MakePath(new double[] { 31, 151 ,41,161}) };
      ppp1.AddRange(ppp2);
      PointD p = new Clipper2Lib.PointD(9, 9);
      PointD p2 = new Clipper2Lib.PointD(99, 99);
      PathD pp = new PathD();
      pp.Add(p);
      pp.Add(p2);
      PathsD ppp3 = new PathsD();
      ppp3.Add(pp);


     // DoRabbit();
     // DoSimpleShapes();      
     // DoVariableOffset();
      DoPeterShapes(dataSets);
    }


    public static void DoPeterShapes(PathsD pp)
    {
      SvgWriter svg = new();

      //TRIANGLE OFFSET - WITH LARGE MITER

      //PathsD pp = new() { Clipper.MakePath(new double[] { 30, 150, 60, 350, 0, 350 }) };
      PathsD solution = new();
      for (int i = 0; i < 5; ++i)
      {
        //nb: the last parameter here (10) greatly increases miter limit
        pp = Clipper.InflatePaths(pp,  5, JoinType.Miter, EndType.Polygon, 10);
        solution.AddRange(pp);
      }
      SvgUtils.AddSolution(svg, solution, false);

      // RECTANGLE OFFSET - BEVEL, SQUARED AND ROUNDED

      solution.Clear();
      solution.Add(Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200 }));
      solution.Add(Clipper.TranslatePath(solution[0], 60, 50));
      solution.Add(Clipper.TranslatePath(solution[1], 60, 50));
      SvgUtils.AddOpenSubject(svg, solution);

      // nb: rather than using InflatePaths(), we have to use the 
      // ClipperOffest class directly because we want to perform
      // different join types in a single offset operation
      ClipperOffset co = new();
      // because ClipperOffset only accepts Int64 paths, scale them 
      // so the de-scaled offset result will have greater precision
      double scale = 100;
      Paths64 pp64 = Clipper.ScalePaths64(solution, scale);
      co.AddPath(pp64[0], JoinType.Bevel, EndType.Joined);
      co.AddPath(pp64[1], JoinType.Square, EndType.Joined);
      co.AddPath(pp64[2], JoinType.Round, EndType.Joined);
      co.Execute(10 * scale, pp64);
      // now de-scale the offset solution
      solution = Clipper.ScalePathsD(pp64, 1 / scale);

      const string filename = "../../../inflate.svg";
      SvgUtils.AddSolution(svg, solution, false);
      SvgUtils.AddCaption(svg, "Beveled join", 100, -17);
      SvgUtils.AddCaption(svg, "Squared join", 160, 33);
      SvgUtils.AddCaption(svg, "Rounded join", 220, 83);
      SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 800, 600, 40);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoSimpleShapes()
    {
      SvgWriter svg = new();

      //TRIANGLE OFFSET - WITH LARGE MITER

      PathsD pp = new() { Clipper.MakePath(new double[] { 30,150, 60,350, 0,350 }) };
      PathsD solution = new();
      for (int i = 0; i < 5; ++i)
      {
        //nb: the last parameter here (10) greatly increases miter limit
        pp = Clipper.InflatePaths(pp, 5, JoinType.Miter, EndType.Polygon, 10);
        solution.AddRange(pp);
      }
      SvgUtils.AddSolution(svg, solution, false);

      // RECTANGLE OFFSET - BEVEL, SQUARED AND ROUNDED

      solution.Clear();
      solution.Add(Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200 }));
      solution.Add(Clipper.TranslatePath(solution[0], 60, 50));
      solution.Add(Clipper.TranslatePath(solution[1], 60, 50));
      SvgUtils.AddOpenSubject(svg, solution);

      // nb: rather than using InflatePaths(), we have to use the 
      // ClipperOffest class directly because we want to perform
      // different join types in a single offset operation
      ClipperOffset co = new();
      // because ClipperOffset only accepts Int64 paths, scale them 
      // so the de-scaled offset result will have greater precision
      double scale = 100;
      Paths64 pp64 = Clipper.ScalePaths64(solution, scale);
      co.AddPath(pp64[0], JoinType.Bevel, EndType.Joined);
      co.AddPath(pp64[1], JoinType.Square, EndType.Joined);
      co.AddPath(pp64[2], JoinType.Round, EndType.Joined);
      co.Execute(10 * scale, pp64);
      // now de-scale the offset solution
      solution = Clipper.ScalePathsD(pp64, 1 / scale);

      const string filename = "../../../inflate.svg";
      SvgUtils.AddSolution(svg, solution, false);
      SvgUtils.AddCaption(svg, "Beveled join", 100, -17);
      SvgUtils.AddCaption(svg, "Squared join", 160, 33);
      SvgUtils.AddCaption(svg, "Rounded join", 220, 83);
      SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 800, 600, 40);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }


    public static void DisplaySolutionAsSvg(string filename, PathsD solution)
    {
      SvgWriter svg = new();
      SvgUtils.AddSolution(svg, solution, false);
      SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 450, 720, 10);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoRabbit()
    {
      if (!File.Exists("..\\..\\..\\rabbit.svg")) return;
      SvgReader sr = new("..\\..\\..\\rabbit.svg");
      PathsD pp  = sr.Paths;
      PathsD solution = new (pp);
      while (pp.Count > 0)
      {
        pp = Clipper.InflatePaths(pp, -5, JoinType.Round, EndType.Polygon);
        // SimplifyPaths - is recommended here as it removes tiny 
        // offsetting artefacts and speeds up this while loop
        pp = Clipper.SimplifyPaths(pp, 0.25);
        solution.AddRange(pp);
      }
      DisplaySolutionAsSvg("..\\..\\..\\rabbit_inflate.svg", solution);
    }
    public static void DoVariableOffset()
    {
      Paths64 p = new() { Clipper.MakePath(new int[] { 0,50, 20,50, 40,50, 60,50, 80,50, 100,50 }) };
      Paths64 solution = new();
      ClipperOffset co = new();
      co.AddPaths(p, JoinType.Square, EndType.Butt);
      co.Execute(
        (path, path_norms, currPt, prevPt) => currPt * currPt + 10, solution);

      const string filename = "../../../variable_offset.svg";
      SvgWriter svg = new();
      SvgUtils.AddOpenSubject(svg, p);
      SvgUtils.AddSolution(svg, solution, true);
      SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 500, 500, 60);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

  } //end Application

} //namespace
